using Botje.Core;
using Botje.DB;
using Botje.Messaging.Events;
using Botje.Messaging.Models;
using Ninject;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Botje.Messaging.Telegram
{
    public class TelegramClient : IMessagingClient
    {
        private readonly TimeSpan GetMeTimeout = TimeSpan.FromSeconds(5);
        private readonly TimeSpan SendMessageToChatTimeout = TimeSpan.FromSeconds(5);
        private readonly TimeSpan AnswerCallbackQueryTimeout = TimeSpan.FromSeconds(5);
        private readonly TimeSpan EditMessageTextTimeout = TimeSpan.FromSeconds(30);
        private readonly TimeSpan AnswerInlineQueryTimeout = TimeSpan.FromSeconds(5);
        private readonly TimeSpan DefaultActionTimeout = TimeSpan.FromSeconds(5);
#if DEBUG
        private readonly TimeSpan MessageProcessingTimeout = TimeSpan.FromHours(24);
#else
        private readonly TimeSpan MessageProcessingTimeout = TimeSpan.FromSeconds(37);
#endif

        private readonly TimeSpan MessageProcessingErrorFixedDelay = TimeSpan.FromMilliseconds(503);
        private readonly TimeSpan MessageProcessingMinimumPollingInterval = TimeSpan.FromMilliseconds(1009);

        public event EventHandler<PrivateMessageEventArgs> OnPrivateMessage;
        public event EventHandler<PublicMessageEventArgs> OnPublicMessage;
        public event EventHandler<InlineQueryEventArgs> OnInlineQuery;
        public event EventHandler<QueryCallbackEventArgs> OnQueryCallback;
        public event EventHandler<PrivateMessageEditedEventArgs> OnPrivateMessageEdited;
        public event EventHandler<PublicMessageEditedEventArgs> OnPublicMessageEdited;
        public event EventHandler<ChannelMessageEventArgs> OnChannelMessage;
        public event EventHandler<ChosenInlineQueryResultEventArgs> OnChosenInlineQueryResult;
        public event EventHandler<ChannelMessageEditedEventArgs> OnChannelMessageEdited;

        [Inject]
        public ILoggerFactory LoggerFactory { set { Log = value.Create(GetType()); } }

        [Inject]
        public IDatabase Database { get; set; }

        private Thread _updateThread;
        private CancellationToken _cancellationToken;
        protected ILogger Log;
        private RestClient _restClient;
        public String FileBaseURL { get; private set; }

        private string APISerialize(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
        }

        public virtual void Setup(string key, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            FileBaseURL = $"https://api.telegram.org/file/bot{key}";
            _restClient = new RestClient($"https://api.telegram.org/bot{key}");
        }

        public virtual void Start()
        {
            if (null != _updateThread)
            {
                Log.Error($"Can't start the telegram client when it's already running.");
                throw new InvalidOperationException($"An update thread is already running.");
            }
            _updateThread = new Thread(UpdateThreadFunction)
            {
                IsBackground = true
            };
            _updateThread.Start();
        }

        public virtual void Stop()
        {
            if (null == _updateThread)
            {
                Log.Error($"Can't stop the telegram client when it's not running.");
                throw new InvalidOperationException($"The update thread is not running so it can't be stopped.");
            }
            _updateThread.Abort();
            _updateThread = null;
        }

        private void UpdateThreadFunction(object obj)
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var request = new RestRequest("getUpdates", Method.POST);
                    request.AddParameter("offset", GetNextUpdateID());
                    request.AddParameter("limit", 25);
                    request.AddParameter("timeout", 2);
                    request.AddParameter("allowed_updates", new[] { "message", "edited_message", "inline_query", "chosen_inline_result", "callback_query" });

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    Semaphore sem = new Semaphore(0, 1);
                    _restClient.ExecuteAsync<Result<List<GetUpdatesResult>>>(request, (result) =>
                    {
                        if (null == result.Data)
                        {
                            Log.Error($"Failed to parse:\r\n---------------\r\n{result.Content}\r\n---------------\r\nRequest was: {result.Request.Resource}");
                        }
                        else if (result.Data.OK)
                        {
                            if (result.Data.Data?.Count > 0)
                            {
                                Log.Trace($"{request.Resource}/{request.Method} returned {result.Data.Data.Count} results in {sw.ElapsedMilliseconds} milliseconds");
                                foreach (GetUpdatesResult update in result.Data.Data)
                                {
                                    ProcessUpdate(update);
                                    RecordUpdateDone(update);
                                }
                            }
                        }
                        else
                        {
                            Log.Error($"Error in '{request.Resource}/{request.Method} ': Code: \"{result.Data.ErrorCode}\" Description: \"{result.Data.Description}\"");
                        }
                        sem.Release();
                    });
                    if (!sem.WaitOne(MessageProcessingTimeout))
                    {
                        Log.Warn("System took too long to process messages.");
                    }
                    sw.Stop();

                    // Do not ask more often than once every so much time
                    if (sw.Elapsed < MessageProcessingMinimumPollingInterval)
                    {
                        Thread.Sleep(MessageProcessingMinimumPollingInterval - sw.Elapsed);
                    }
                }
                catch (ThreadAbortException)
                {
                    Log.Trace($"Cancellation is requested. Stopping the update-thread.");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Uncaught exception in update thread.");
                    Thread.Sleep(MessageProcessingErrorFixedDelay);
                }
            }
        }

        private object _updateLock = new object();

        private void RecordUpdateDone(GetUpdatesResult update)
        {
            lock (_updateLock)
            {
                var table = Database.GetCollection<TelegramBotState>();
                var record = table.Find(x => true).FirstOrDefault();
                if (null == record)
                {
                    record = new TelegramBotState();
                    table.Insert(record);
                }
                record.LastProcessedUpdateID = update.UpdateID;
                table.Update(record);
            }
        }

        private object GetNextUpdateID()
        {
            lock (_updateLock)
            {
                var table = Database.GetCollection<TelegramBotState>();
                var record = table.Find(x => true).FirstOrDefault();
                return 1 + (record?.LastProcessedUpdateID ?? -2);
            }
        }

        private void ProcessUpdate(GetUpdatesResult update)
        {
            Log.Trace($"Processing update with ID: {update.UpdateID} and type: {update.GetUpdateType()}");

            try
            {
                if (update.CallbackQuery != null)
                {
                    OnQueryCallback?.Invoke(this, new QueryCallbackEventArgs(update.UpdateID, update.CallbackQuery));
                }
                else if (update.ChannelPost != null)
                {
                    OnChannelMessage?.Invoke(this, new ChannelMessageEventArgs(update.UpdateID, update.ChannelPost));
                }
                else if (update.ChosenInlineResult != null)
                {
                    OnChosenInlineQueryResult?.Invoke(this, new ChosenInlineQueryResultEventArgs(update.UpdateID, update.ChosenInlineResult));
                }
                else if (update.EditedChannelPost != null)
                {
                    OnChannelMessageEdited?.Invoke(this, new ChannelMessageEditedEventArgs(update.UpdateID, update.EditedChannelPost));
                }
                else if (update.InlineQuery != null)
                {
                    OnInlineQuery?.Invoke(this, new InlineQueryEventArgs(update.UpdateID, update.InlineQuery));
                }
                else if (update.EditedMessage != null)
                {
                    if (string.Equals(update.EditedMessage.Chat.Type, "private", StringComparison.InvariantCultureIgnoreCase))
                    {
                        OnPrivateMessageEdited?.Invoke(this, new PrivateMessageEditedEventArgs(update.UpdateID, update.EditedMessage));
                    }
                    else
                    {
                        OnPublicMessageEdited?.Invoke(this, new PublicMessageEditedEventArgs(update.UpdateID, update.EditedMessage));
                    }
                }
                else if (update.Message != null)
                {
                    if (string.Equals(update.Message.Chat.Type, "private", StringComparison.InvariantCultureIgnoreCase))
                    {
                        OnPrivateMessage?.Invoke(this, new PrivateMessageEventArgs(update.UpdateID, update.Message));
                    }
                    else
                    {
                        OnPublicMessage?.Invoke(this, new PublicMessageEventArgs(update.UpdateID, update.Message));
                    }
                }
                else
                {
                    Log.Trace($"Not processing message of type {update.GetUpdateType()}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Processing update with ID #{update.UpdateID} failed. Ignoring message.");
            }
        }

        public virtual User GetMe()
        {
            Log.Trace($"Invoked: GetMe()");

            User result = null;

            var request = new RestRequest("getMe", Method.POST);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Semaphore sem = new Semaphore(0, 1);
            _restClient.ExecuteAsync<Result<User>>(request, (restResult) =>
            {
                if (restResult.Data.OK)
                {
                    Log.Trace($"{request.Resource}/{request.Method} returned in {sw.ElapsedMilliseconds} milliseconds");
                    result = restResult.Data.Data;
                }
                else
                {
                    Log.Error($"Error in '{request.Resource}/{request.Method} ': Code: \"{restResult.Data.ErrorCode}\" Description: \"{restResult.Data.Description}\"");
                }
                sem.Release();
            });
            if (!sem.WaitOne(GetMeTimeout))
            {
                string error = $"Timeout waiting for {request.Resource}/{request.Method} after {sw.ElapsedMilliseconds} milliseconds.";
                Log.Error(error);
                throw new TimeoutException(error);
            }
            sw.Stop();

            return result;
        }

        private class SendMessageRequest
        {
            public long chat_id { get; set; }
            public string text { get; set; }
            public string parse_mode { get; set; }
            public bool? disable_web_page_preview { get; set; }
            public bool? disable_notification { get; set; }
            public long? reply_to_message_id { get; set; }
            public InlineKeyboardMarkup reply_markup { get; set; }
        }

        public virtual Message SendMessageToChat(long chatID, string text, string parseMode = null, bool? disableWebPagePreview = null, bool? disableNotification = null, long? replyToMessageId = null, InlineKeyboardMarkup replyMarkup = null)
        {
            Log.Trace($"Invoked: SendMessageToChat(chatID={chatID},text={text},replyMarkup={replyMarkup})");

            Message result = null;

            var request = new RestRequest("sendMessage", Method.POST);

            var parameters = new SendMessageRequest
            {
                chat_id = chatID,
                text = text,
                parse_mode = parseMode,
                disable_notification = disableNotification,
                disable_web_page_preview = disableWebPagePreview,
                reply_to_message_id = replyToMessageId,
                reply_markup = replyMarkup,
            };
            var jsonParams = APISerialize(parameters);
            request.AddParameter("application/json; charset=utf-8", jsonParams, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Semaphore sem = new Semaphore(0, 1);
            _restClient.ExecuteAsync<Result<Message>>(request, (restResult) =>
            {
                if (restResult.Data.OK)
                {
                    result = restResult.Data.Data;
                    Log.Trace($"{request.Resource}/{request.Method} returned in {sw.ElapsedMilliseconds} milliseconds");
                }
                else
                {
                    Log.Error($"Error in '{request.Resource}/{request.Method}': Code: \"{restResult.Data.ErrorCode}\" Description: \"{restResult.Data.Description}\"");
                }
                sem.Release();
            }
            );
            if (!sem.WaitOne(SendMessageToChatTimeout))
            {
                string error = $"Timeout waiting for {request.Resource}/{request.Method} after {sw.ElapsedMilliseconds} milliseconds.";
                Log.Error(error);
                throw new TimeoutException(error);
            }
            sw.Stop();
            return result;
        }

        private class AnswerCallbackQueryParams
        {
            public string callback_query_id { get; set; }
            public string text { get; set; }
            public bool? show_alert { get; set; }
        }

        public virtual bool AnswerCallbackQuery(string callbackQueryID, string text = null, bool? showAlert = false)
        {
            Log.Trace($"Invoked: AnswerCallbackQuery(callbackQueryID={callbackQueryID},text={text})");

            bool result = false;

            var request = new RestRequest("answerCallbackQuery", Method.POST);

            var parameters = new AnswerCallbackQueryParams
            {
                callback_query_id = callbackQueryID,
                show_alert = showAlert,
                text = text,
            };
            var jsonParams = APISerialize(parameters);
            request.AddParameter("application/json; charset=utf-8", jsonParams, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Semaphore sem = new Semaphore(0, 1);
            _restClient.ExecuteAsync<Result<bool>>(request, (restResult) =>
            {
                if (restResult.Data.OK)
                {
                    result = restResult.Data.Data;
                    Log.Trace($"{request.Resource}/{request.Method} returned in {sw.ElapsedMilliseconds} milliseconds");
                }
                else
                {
                    Log.Error($"Error in '{request.Resource}/{request.Method}': Code: \"{restResult.Data.ErrorCode}\" Description: \"{restResult.Data.Description}\"");
                }
                sem.Release();
            }
            );
            if (!sem.WaitOne(AnswerCallbackQueryTimeout))
            {
                string error = $"Timeout waiting for {request.Resource}/{request.Method} after {sw.ElapsedMilliseconds} milliseconds.";
                Log.Error(error);
                throw new TimeoutException(error);
            }
            sw.Stop();
            return result;
        }

        private class EditMessageTextParams
        {
            public string chat_id { get; set; }
            public long? message_id { get; set; }
            public string inline_message_id { get; set; }
            public string text { get; set; }
            public string parse_mode { get; set; }
            public bool? disable_web_page_preview { get; set; }
            public InlineKeyboardMarkup reply_markup { get; set; }
        }

        public virtual void EditMessageText(string chatID, long? messageID, string inlineMessageID, string text, string parseMode = null, bool? disableWebPagePreview = null, InlineKeyboardMarkup replyMarkup = null, string chatType = "private")
        {
            Log.Trace($"Invoked: EditMessageText(chatID={chatID},messageID={messageID},inlineMessageID={inlineMessageID},text={text},replyMarkup={replyMarkup})");

            var request = new RestRequest("editMessageText", Method.POST);

            var parameters = new EditMessageTextParams
            {
                chat_id = chatID,
                message_id = messageID,
                inline_message_id = inlineMessageID,
                text = text,
                parse_mode = parseMode,
                disable_web_page_preview = disableWebPagePreview,
                reply_markup = replyMarkup,
            };
            var jsonParams = APISerialize(parameters);
            request.AddParameter("application/json; charset=utf-8", jsonParams, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Semaphore sem = new Semaphore(0, 1);
            _restClient.ExecuteAsync<Result>(request, (restResult) =>
            {
                if (restResult.Data.OK)
                {
                    Log.Trace($"{request.Resource}/{request.Method} returned in {sw.ElapsedMilliseconds} milliseconds");
                }
                else
                {
                    Log.Error($"Error in '{request.Resource}/{request.Method}': Code: \"{restResult.Data.ErrorCode}\" Description: \"{restResult.Data.Description}\"");
                }
                sem.Release();
            }
            );
            if (!sem.WaitOne(EditMessageTextTimeout))
            {
                string error = $"Timeout waiting for {request.Resource}/{request.Method} after {sw.ElapsedMilliseconds} milliseconds.";
                Log.Error(error);
                throw new TimeoutException(error);
            }
            sw.Stop();
        }

        private class AnswerInlineQueryParams
        {
            public string inline_query_id { get; set; }
            public List<InlineQueryResultArticle> results { get; set; }
            public int? cache_time { get; set; }
            public bool? is_personal { get; set; }
            public string next_offset { get; set; }
            public string switch_pm_text { get; set; }
            public string switch_pm_parameter { get; set; }
        }

        public virtual void AnswerInlineQuery(string queryID, List<InlineQueryResultArticle> results)
        {
            Log.Trace($"Invoked: AnswerInlineQuery(queryID={queryID},results={results})");

            var request = new RestRequest("answerInlineQuery", Method.POST);

            var parameters = new AnswerInlineQueryParams
            {
                inline_query_id = queryID,
                results = results,
                cache_time = 10,
                is_personal = false,
            };
            var jsonParams = APISerialize(parameters);
            request.AddParameter("application/json; charset=utf-8", jsonParams, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Semaphore sem = new Semaphore(0, 1);
            _restClient.ExecuteAsync<Result>(request, (restResult) =>
            {
                if (restResult.Data.OK)
                {
                    Log.Trace($"{request.Resource}/{request.Method} returned in {sw.ElapsedMilliseconds} milliseconds");
                }
                else
                {
                    Log.Error($"Error in '{request.Resource}/{request.Method}': Code: \"{restResult.Data.ErrorCode}\" Description: \"{restResult.Data.Description}\"");
                }
                sem.Release();
            }
            );
            if (!sem.WaitOne(AnswerInlineQueryTimeout))
            {
                string error = $"Timeout waiting for {request.Resource}/{request.Method} after {sw.ElapsedMilliseconds} milliseconds.";
                Log.Error(error);
                throw new TimeoutException(error);
            }
            sw.Stop();
        }
        private class ForwardMessageParams
        {
            public string chat_id { get; set; }
            public string from_chat_id { get; set; }
            public bool? disable_notification { get; set; }
            public long message_id { get; set; }
        }

        public virtual void ForwardMessageToChat(long chatID, long sourceChat, long sourceMessageID)
        {
            Log.Trace($"Invoked: ForwardMessageToChat(chatID={chatID},sourceChat={sourceChat},sourceMessageID={sourceMessageID})");

            var request = new RestRequest("forwardMessage", Method.POST);

            var parameters = new ForwardMessageParams
            {
                chat_id = $"{chatID}",
                from_chat_id = $"{sourceChat}",
                disable_notification = null,
                message_id = sourceMessageID
            };
            var jsonParams = APISerialize(parameters);
            request.AddParameter("application/json; charset=utf-8", jsonParams, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Semaphore sem = new Semaphore(0, 1);
            _restClient.ExecuteAsync<Result>(request, (restResult) =>
            {
                if (restResult.Data.OK)
                {
                    Log.Trace($"{request.Resource}/{request.Method} returned in {sw.ElapsedMilliseconds} milliseconds");
                }
                else
                {
                    Log.Error($"Error in '{request.Resource}/{request.Method} ': Code: \"{restResult.Data.ErrorCode}\" Description: \"{restResult.Data.Description}\"");
                }
                sem.Release();
            }
            );
            if (!sem.WaitOne(DefaultActionTimeout))
            {
                string error = $"Timeout waiting for {request.Resource}/{request.Method} after {sw.ElapsedMilliseconds} milliseconds.";
                Log.Error(error);
                throw new TimeoutException(error);
            }
            sw.Stop();
        }

        private class GetFileParams
        {
            public string file_id { get; set; }
        }

        public virtual Models.File GetFile(string fileID)
        {
            Log.Trace($"Invoked: GetFile(fileID={fileID})");

            Models.File result = null;

            var request = new RestRequest("getFile", Method.POST);

            var parameters = new GetFileParams
            {
                file_id = fileID,
            };
            var jsonParams = APISerialize(parameters);
            request.AddParameter("application/json; charset=utf-8", jsonParams, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Semaphore sem = new Semaphore(0, 1);
            _restClient.ExecuteAsync<Result<Models.File>>(request, (restResult) =>
            {
                if (restResult.Data.OK)
                {
                    result = restResult.Data.Data;
                    Log.Trace($"{request.Resource}/{request.Method} returned in {sw.ElapsedMilliseconds} milliseconds");
                }
                else
                {
                    Log.Error($"Error in '{request.Resource}/{request.Method}': Code: \"{restResult.Data.ErrorCode}\" Description: \"{restResult.Data.Description}\"");
                }
                sem.Release();
            }
            );
            if (!sem.WaitOne(SendMessageToChatTimeout))
            {
                string error = $"Timeout waiting for {request.Resource}/{request.Method} after {sw.ElapsedMilliseconds} milliseconds.";
                Log.Error(error);
                throw new TimeoutException(error);
            }
            sw.Stop();
            return result;
        }

        private class GetChatParams
        {
            public string chat_id { get; set; }
        }

        public virtual Chat GetChat(long chatID)
        {
            Log.Trace($"Invoked: GetChat(chatID={chatID}");

            var request = new RestRequest("getChat", Method.POST);

            var parameters = new GetChatParams
            {
                chat_id = $"{chatID}",
            };
            var jsonParams = APISerialize(parameters);
            request.AddParameter("application/json; charset=utf-8", jsonParams, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Chat result = null;

            Semaphore sem = new Semaphore(0, 1);
            _restClient.ExecuteAsync<Result<Chat>>(request, (restResult) =>
            {
                if (restResult.Data.OK)
                {
                    result = restResult.Data.Data;
                    Log.Trace($"{request.Resource}/{request.Method} returned in {sw.ElapsedMilliseconds} milliseconds");
                }
                else
                {
                    Log.Error($"Error in '{request.Resource}/{request.Method} ': Code: \"{restResult.Data.ErrorCode}\" Description: \"{restResult.Data.Description}\"");
                }
                sem.Release();
                sw.Stop();
            }
            );
            if (!sem.WaitOne(DefaultActionTimeout))
            {
                string error = $"Timeout waiting for {request.Resource}/{request.Method} after {sw.ElapsedMilliseconds} milliseconds.";
                Log.Error(error);
                throw new TimeoutException(error);
            }

            return result;
        }

        private class DeleteMessageParams
        {
            public string chat_id { get; set; }
            public long message_id { get; set; }
        }

        public virtual bool DeleteMessage(long chatID, long messageID)
        {
            Log.Trace($"Invoked: DeleteMessage(chatID={chatID}, messageID={messageID})");

            var request = new RestRequest("deleteMessage", Method.POST);

            var parameters = new DeleteMessageParams
            {
                chat_id = $"{chatID}",
                message_id = messageID
            };
            var jsonParams = APISerialize(parameters);
            request.AddParameter("application/json; charset=utf-8", jsonParams, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            bool result = default(bool);

            Semaphore sem = new Semaphore(0, 1);
            _restClient.ExecuteAsync<Result<bool>>(request, (restResult) =>
            {
                if (restResult.Data.OK)
                {
                    result = restResult.Data.Data;
                    Log.Trace($"{request.Resource}/{request.Method} returned in {sw.ElapsedMilliseconds} milliseconds");
                }
                else
                {
                    Log.Error($"Error in '{request.Resource}/{request.Method} ': Code: \"{restResult.Data.ErrorCode}\" Description: \"{restResult.Data.Description}\"");
                }
                sem.Release();
                sw.Stop();
            }
            );
            if (!sem.WaitOne(DefaultActionTimeout))
            {
                string error = $"Timeout waiting for {request.Resource}/{request.Method} after {sw.ElapsedMilliseconds} milliseconds.";
                Log.Error(error);
                throw new TimeoutException(error);
            }

            return result;
        }

        public class KickChatMemberParams
        {
            public string chat_id { get; set; }
            public string user_id { get; set; }
            public string until_date { get; set; }
        }

        /// <summary>
        /// Kicks the user from the chat. Use a unix timestamp to specify an until-date or time
        /// </summary>
        /// <param name="chatID"></param>
        /// <param name="userID"></param>
        /// <param name="untilDate"></param>
        public virtual void KickChatMember(long chatID, long userID, DateTimeOffset? untilDate)
        {
            Log.Trace($"Invoked: KickChatMember(chatID={chatID}, messageID={userID}, untilDate={untilDate})");

            var request = new RestRequest("kickChatMember", Method.POST);

            var parameters = new KickChatMemberParams
            {
                chat_id = $"{chatID}",
                user_id = $"{userID}",
                until_date = untilDate.HasValue ? $"{untilDate.Value.ToUnixTimeSeconds()}" : null,

            };
            var jsonParams = APISerialize(parameters);
            request.AddParameter("application/json; charset=utf-8", jsonParams, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Semaphore sem = new Semaphore(0, 1);
            _restClient.ExecuteAsync<Result<int>>(request, (restResult) =>
            {
                if (restResult.Data.OK)
                {
                    Log.Trace($"{request.Resource}/{request.Method} returned in {sw.ElapsedMilliseconds} milliseconds");
                }
                else
                {
                    Log.Error($"Error in '{request.Resource}/{request.Method} ': Code: \"{restResult.Data.ErrorCode}\" Description: \"{restResult.Data.Description}\"");
                }
                sem.Release();
                sw.Stop();
            }
            );
            if (!sem.WaitOne(DefaultActionTimeout))
            {
                string error = $"Timeout waiting for {request.Resource}/{request.Method} after {sw.ElapsedMilliseconds} milliseconds.";
                Log.Error(error);
                throw new TimeoutException(error);
            }
        }

        public virtual Message SendDocument(long chatID, Stream document, string filename, string contentType, long contentLength, string caption, string parseMode = "HTML", bool? disableNotification = null, int? replyToMessageId = null, InlineKeyboardMarkup replyMarkup = null)
        {
            Log.Trace($"Invoked: SendDocument(chatID={chatID},caption={caption},replyMarkup={replyMarkup})");

            Message result = null;

            var request = new RestRequest("sendDocument", Method.POST);

            request.AddParameter("chat_id", $"{chatID}");
            request.AddParameter("caption", caption);
            if (parseMode != null) request.AddParameter("parse_mode", parseMode);
            if (disableNotification.HasValue) request.AddParameter("disable_notification", disableNotification);
            if (replyToMessageId.HasValue) request.AddParameter("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) request.AddParameter("reply_markup", APISerialize(replyMarkup));
            request.AddFile("document", document.CopyTo, filename, contentLength, contentType);
            request.AddHeader("Content-Type", "multipart/form-data");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Semaphore sem = new Semaphore(0, 1);
            _restClient.ExecuteAsync<Result<Message>>(request, (restResult) =>
            {
                if (restResult.Data.OK)
                {
                    result = restResult.Data.Data;
                    Log.Trace($"{request.Resource}/{request.Method} returned in {sw.ElapsedMilliseconds} milliseconds");
                }
                else
                {
                    Log.Error($"Error in '{request.Resource}/{request.Method}': Code: \"{restResult.Data.ErrorCode}\" Description: \"{restResult.Data.Description}\"");
                }
                sem.Release();
            }
            );
            if (!sem.WaitOne(SendMessageToChatTimeout))
            {
                string error = $"Timeout waiting for {request.Resource}/{request.Method} after {sw.ElapsedMilliseconds} milliseconds.";
                Log.Error(error);
                throw new TimeoutException(error);
            }
            sw.Stop();
            return result;
        }

        public virtual Message SendPhoto(long chatID, Stream photo, string filename, string contentType, long contentLength, string caption, string parseMode = "HTML", bool? disableNotification = null, int? replyToMessageId = null, InlineKeyboardMarkup replyMarkup = null)
        {
            Log.Trace($"Invoked: SendPhoto(chatID={chatID},caption={caption},replyMarkup={replyMarkup})");

            Message result = null;

            var request = new RestRequest("sendPhoto", Method.POST);

            request.AddParameter("chat_id", $"{chatID}");
            request.AddParameter("caption", caption);
            if (parseMode != null) request.AddParameter("parse_mode", parseMode);
            if (disableNotification.HasValue) request.AddParameter("disable_notification", disableNotification);
            if (replyToMessageId.HasValue) request.AddParameter("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) request.AddParameter("reply_markup", APISerialize(replyMarkup));
            request.AddFile("photo", photo.CopyTo, filename, contentLength, contentType);
            request.AddHeader("Content-Type", "multipart/form-data");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Semaphore sem = new Semaphore(0, 1);
            _restClient.ExecuteAsync<Result<Message>>(request, (restResult) =>
            {
                if (restResult.Data.OK)
                {
                    result = restResult.Data.Data;
                    Log.Trace($"{request.Resource}/{request.Method} returned in {sw.ElapsedMilliseconds} milliseconds");
                }
                else
                {
                    Log.Error($"Error in '{request.Resource}/{request.Method}': Code: \"{restResult.Data.ErrorCode}\" Description: \"{restResult.Data.Description}\"");
                }
                sem.Release();
            }
            );
            if (!sem.WaitOne(SendMessageToChatTimeout))
            {
                string error = $"Timeout waiting for {request.Resource}/{request.Method} after {sw.ElapsedMilliseconds} milliseconds.";
                Log.Error(error);
                throw new TimeoutException(error);
            }
            sw.Stop();
            return result;
        }

        public virtual Message SendAudio(long chatID, Stream audio, string filename, string contentType, long contentLength, long? durationInSeconds = null, string performer = null, string title = null, string caption = null, string parseMode = "HTML", bool? disableNotification = null, int? replyToMessageId = null, InlineKeyboardMarkup replyMarkup = null)
        {
            Log.Trace($"Invoked: SendPhoto(chatID={chatID},caption={caption},replyMarkup={replyMarkup})");

            Message result = null;

            var request = new RestRequest("sendAudio", Method.POST);

            request.AddParameter("chat_id", $"{chatID}");
            if (caption != null) request.AddParameter("caption", caption);
            if (parseMode != null) request.AddParameter("parse_mode", parseMode);
            if (disableNotification.HasValue) request.AddParameter("disable_notification", disableNotification);
            if (replyToMessageId.HasValue) request.AddParameter("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) request.AddParameter("reply_markup", APISerialize(replyMarkup));

            if (durationInSeconds.HasValue) request.AddParameter("duration", durationInSeconds.Value);
            if (performer != null) request.AddParameter("performer", performer);
            if (title != null) request.AddParameter("title", title);
            request.AddFile("audio", audio.CopyTo, filename, contentLength, contentType);
            request.AddHeader("Content-Type", "multipart/form-data");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Semaphore sem = new Semaphore(0, 1);
            _restClient.ExecuteAsync<Result<Message>>(request, (restResult) =>
            {
                if (restResult.Data.OK)
                {
                    result = restResult.Data.Data;
                    Log.Trace($"{request.Resource}/{request.Method} returned in {sw.ElapsedMilliseconds} milliseconds");
                }
                else
                {
                    Log.Error($"Error in '{request.Resource}/{request.Method}': Code: \"{restResult.Data.ErrorCode}\" Description: \"{restResult.Data.Description}\"");
                }
                sem.Release();
            }
            );
            if (!sem.WaitOne(SendMessageToChatTimeout))
            {
                string error = $"Timeout waiting for {request.Resource}/{request.Method} after {sw.ElapsedMilliseconds} milliseconds.";
                Log.Error(error);
                throw new TimeoutException(error);
            }
            sw.Stop();
            return result;
        }
    }
}
