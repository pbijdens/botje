﻿using Botje.Core;
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

        private void ProcessMessage(Models.Message message)
        {
            string announcement = $"MessageID={message.MessageID}, ChatID={message.Chat.ID}, ReplyToID={message.ReplyToMessage?.MessageID}, ForwardedMessage={message.ForwardFromMessageId}, FromID={message.From?.ID}, FromName={message.From?.UsernameOrName()}, Text={message.Text?.Substring(0, Math.Min(10, message.Text?.Length ?? 0))}";
            _log.Trace(announcement);
            Client.SendMessageToChat(message.Chat.ID, announcement);
        }
    }
}