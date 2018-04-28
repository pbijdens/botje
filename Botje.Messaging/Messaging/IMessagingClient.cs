using Botje.Messaging.Events;
using Botje.Messaging.Models;
using System;
using System.Collections.Generic;

namespace Botje.Messaging
{
    /// <summary>
    /// Messaging client interface.
    /// </summary>
    public interface IMessagingClient
    {
        /// <summary>
        /// Start receiving.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop receiving.
        /// </summary>
        void Stop();

        /// <summary>
        /// Private message was received.
        /// </summary>
        event EventHandler<PrivateMessageEventArgs> OnPrivateMessage;

        /// <summary>
        /// Public message was received.
        /// </summary>
        event EventHandler<PublicMessageEventArgs> OnPublicMessage;

        /// <summary>
        /// A message arrived in a channel.
        /// </summary>
        event EventHandler<ChannelMessageEventArgs> OnChannelMessage;

        /// <summary>
        /// Inline query was requested.
        /// </summary>
        event EventHandler<InlineQueryEventArgs> OnInlineQuery;

        /// <summary>
        /// Inline query was requested.
        /// </summary>
        event EventHandler<ChosenInlineQueryResultEventArgs> OnChosenInlineQueryResult;

        /// <summary>
        /// Query callback reply was received.
        /// </summary>
        event EventHandler<QueryCallbackEventArgs> OnQueryCallback;

        /// <summary>
        /// A private message was edited.
        /// </summary>
        event EventHandler<ChannelMessageEditedEventArgs> OnChannelMessageEdited;

        /// <summary>
        /// A private message was edited.
        /// </summary>
        event EventHandler<PrivateMessageEditedEventArgs> OnPrivateMessageEdited;

        /// <summary>
        /// A public message was edited.
        /// </summary>
        event EventHandler<PublicMessageEditedEventArgs> OnPublicMessageEdited;

        /// <summary>
        /// https://core.telegram.org/bots/api#getme
        /// </summary>
        /// <returns></returns>
        User GetMe();

        /// <summary>
        /// https://core.telegram.org/bots/api#sendmessage
        /// </summary>
        /// <param name="chatID"></param>
        /// <param name="text"></param>
        /// <param name="parseMode"></param>
        /// <param name="disableWebPagePreview"></param>
        /// <param name="disableNotification"></param>
        /// <param name="replyToMessageId"></param>
        /// <param name="replyMarkup"></param>
        /// <returns></returns>
        Message SendMessageToChat(long chatID, string text, string parseMode = "HTML", bool? disableWebPagePreview = null, bool? disableNotification = null, long? replyToMessageId = null, InlineKeyboardMarkup replyMarkup = null);

        /// <summary>
        /// https://core.telegram.org/bots/api#answercallbackquery
        /// </summary>
        /// <param name="callbackQueryID"></param>
        /// <param name="text"></param>
        /// <param name="showAlert"></param>
        /// <returns></returns>
        bool AnswerCallbackQuery(string callbackQueryID, string text = null, bool? showAlert = false);

        /// <summary>
        /// https://core.telegram.org/bots/api#editmessagetext
        /// </summary>
        /// <param name="chatID"></param>
        /// <param name="messageID"></param>
        /// <param name="inlineMessageID"></param>
        /// <param name="text"></param>
        /// <param name="parseMode"></param>
        /// <param name="disableWebPagePreview"></param>
        /// <param name="replyMarkup"></param>
        /// <param name="chatType"></param>
        void EditMessageText(string chatID, long? messageID, string inlineMessageID, string text, string parseMode = null, bool? disableWebPagePreview = null, InlineKeyboardMarkup replyMarkup = null, string chatType = "private");

        /// <summary>
        /// https://core.telegram.org/bots/api#answerinlinequery
        /// </summary>
        /// <param name="queryID"></param>
        /// <param name="results"></param>
        void AnswerInlineQuery(string queryID, List<InlineQueryResultArticle> results);

        /// <summary>
        /// https://core.telegram.org/bots/api#forwardmessage
        /// </summary>
        /// <param name="chatID"></param>
        /// <param name="sourceChat"></param>
        /// <param name="sourceMessageID"></param>
        void ForwardMessageToChat(long chatID, long sourceChat, long sourceMessageID);

        /// <summary>
        /// https://core.telegram.org/bots/api#deletemessage
        /// </summary>
        /// <param name="chatID"></param>
        /// <param name="messageID"></param>
        /// <returns>true upon success</returns>
        bool DeleteMessage(long chatID, long messageID);

        /// <summary>
        /// https://core.telegram.org/bots/api#getfile
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns></returns>
        File GetFile(string fileID);

        /// <summary>
        /// Gets the host/key combination to be used as the base URL for file downloads.
        /// </summary>
        string FileBaseURL { get; }

        /// <summary>
        /// https://core.telegram.org/bots/api#getchat
        /// </summary>
        /// <param name="chatID"></param>
        /// <returns></returns>
        Chat GetChat(long chatID);

        /// <summary>
        /// https://core.telegram.org/bots/api#kickchatmember
        /// </summary>
        /// <param name="chatID"></param>
        /// <param name="userID"></param>
        /// <param name="untilDate"></param>
        void KickChatMember(long chatID, long userID, DateTimeOffset? untilDate);

        /// <summary>
        /// https://core.telegram.org/bots/api#senddocument
        /// </summary>
        /// <param name="chatID"></param>
        /// <param name="document"></param>
        /// <param name="filename"></param>
        /// <param name="contentType"></param>
        /// <param name="contentLength"></param>
        /// <param name="caption"></param>
        /// <param name="parseMode"></param>
        /// <param name="disableNotification"></param>
        /// <param name="replyToMessageId"></param>
        /// <param name="replyMarkup"></param>
        /// <returns></returns>
        Message SendDocument(long chatID, System.IO.Stream document, string filename, string contentType, long contentLength, string caption, string parseMode = "HTML", bool? disableNotification = null, int? replyToMessageId = null, InlineKeyboardMarkup replyMarkup = null);

        /// <summary>
        /// https://core.telegram.org/bots/api#sendphoto
        /// </summary>
        /// <param name="chatID"></param>
        /// <param name="photo"></param>
        /// <param name="filename"></param>
        /// <param name="contentType"></param>
        /// <param name="contentLength"></param>
        /// <param name="caption"></param>
        /// <param name="parseMode"></param>
        /// <param name="disableNotification"></param>
        /// <param name="replyToMessageId"></param>
        /// <param name="replyMarkup"></param>
        /// <returns></returns>
        Message SendPhoto(long chatID, System.IO.Stream photo, string filename, string contentType, long contentLength, string caption, string parseMode = "HTML", bool? disableNotification = null, int? replyToMessageId = null, InlineKeyboardMarkup replyMarkup = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatID"></param>
        /// <param name="audio"></param>
        /// <param name="filename"></param>
        /// <param name="contentType"></param>
        /// <param name="contentLength"></param>
        /// <param name="durationInSeconds"></param>
        /// <param name="performer"></param>
        /// <param name="title"></param>
        /// <param name="caption"></param>
        /// <param name="parseMode"></param>
        /// <param name="disableNotification"></param>
        /// <param name="replyToMessageId"></param>
        /// <param name="replyMarkup"></param>
        /// <returns></returns>
        Message SendAudio(long chatID, System.IO.Stream audio, string filename, string contentType, long contentLength, long? durationInSeconds = null, string performer = null, string title = null, string caption = null, string parseMode = "HTML", bool? disableNotification = null, int? replyToMessageId = null, InlineKeyboardMarkup replyMarkup = null);
    }
}
