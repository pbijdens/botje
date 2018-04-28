using Botje.Core;
using Botje.Core.Utils;
using Botje.Messaging;
using Botje.Messaging.Events;
using Ninject;
using System;
using Models = Botje.Messaging.Models;

namespace Botje.Sample.Modules
{
    /// <summary>
    /// Responds to the /Whoami command. Informs the user about the chat they are in.
    /// </summary>
    public class AnnounceAllMessages : IBotModule
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

        protected readonly Func<string, string> _ = (s) => MessageUtils.HtmlEscape(s);

        private void ProcessMessage(Models.Message message)
        {
            if (message.From.IsBot) return; // not interested in bot messages
            string announcement = "<b>Message: </b> " + _(message.ToString());
            _log.Trace(announcement);
            Client.SendMessageToChat(message.Chat.ID, announcement);
        }
    }
}
