using Botje.Core;
using Botje.Core.Utils;
using Botje.Messaging;
using Botje.Messaging.Events;
using Ninject;
using System;
using System.Linq;
using Models = Botje.Messaging.Models;

namespace Botje.Sample.Modules
{
    /// <summary>
    /// Responds to the /KickChatMember command. Informs the user about the chat they are in.
    /// </summary>
    public class KickChatMember : IBotModule
    {
        private ILogger _log;

        /// <summary></summary>
        [Inject]
        public IMessagingClient Client { get; set; }

        /// <summary></summary>
        [Inject]
        public ILoggerFactory LoggerFactory { set { _log = value.Create(GetType()); } }

        /// <summary>
        /// </summary>
        public void Shutdown()
        {
            Client.OnPrivateMessage -= Client_OnPrivateMessage;
            Client.OnChannelMessage -= Client_OnChannelMessage;
            Client.OnPublicMessage -= Client_OnPublicMessage;
            _log.Trace($"Shut down {GetType().Name}");
        }

        /// <summary>
        /// </summary>
        public void Startup()
        {
            _log.Trace($"Started {GetType().Name}");
            Client.OnPrivateMessage += Client_OnPrivateMessage;
            Client.OnChannelMessage += Client_OnChannelMessage;
            Client.OnPublicMessage += Client_OnPublicMessage;
        }

        private void Client_OnPublicMessage(object sender, PublicMessageEventArgs e)
        {
            ProcessMessage(e.Message);
        }

        private void Client_OnChannelMessage(object sender, ChannelMessageEventArgs e)
        {
            ProcessMessage(e.Message);
        }

        private void Client_OnPrivateMessage(object sender, PrivateMessageEventArgs e)
        {
            ProcessMessage(e.Message);
        }

        private void ProcessMessage(Models.Message message)
        {
            var me = Client.GetMe();
            var firstEntity = message?.Entities?.FirstOrDefault();
            if (null != firstEntity && firstEntity.Type == "bot_command" && firstEntity.Offset == 0)
            {
                string myName = Client.GetMe().Username;
                string commandText = message.Text.Substring(firstEntity.Offset, firstEntity.Length);
                if (commandText.Contains("@") && !commandText.EndsWith($"@{myName}", StringComparison.InvariantCultureIgnoreCase))
                {
                    // not for me
                    _log.Trace($"Got command '{commandText}' but it is not for me.");
                }
                else
                {
                    commandText = commandText.Split("@").First();
                    ProcessCommand(message, commandText);
                }
            }
        }

        private void ProcessCommand(Models.Message message, string command)
        {
            command = command?.ToLower() ?? String.Empty;
            string[] args = message.Text.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Skip(1).ToArray();

            var id = message.Chat.ID;

            switch (command.ToLowerInvariant())
            {
                case "/kick":
                    CmdKickChatMember(message, args);
                    break;
            }
        }

        private void CmdKickChatMember(Models.Message message, string[] args)
        {
            if (args.Length != 1)
            {
                Client.SendMessageToChat(message.Chat.ID, $"@{MessageUtils.HtmlEscape(message.From.UsernameOrName())} use /kick &lt;userid&gt;", "HTML", null, null, message.MessageID);
                return;
            }
            if (long.TryParse(args[0], out long userID))
            {
                if (userID == Client.GetMe().ID)
                {
                    Client.SendMessageToChat(message.Chat.ID, $"Why me?!?! What did I ever do to you?!?", "HTML", null, null, message.MessageID);
                }
                else
                {
                    Client.KickChatMember(message.Chat.ID, userID, DateTimeOffset.UtcNow + TimeSpan.FromHours(1));
                    Client.SendMessageToChat(message.Chat.ID, $"@{MessageUtils.HtmlEscape(message.From.UsernameOrName())} kicked the user...", "HTML", null, null, message.MessageID);
                }
            }
            else
            {
                Client.SendMessageToChat(message.Chat.ID, $"@{MessageUtils.HtmlEscape(message.From.UsernameOrName())} who?!? why?!?", "HTML", null, null, message.MessageID);
            }
        }
    }
}
