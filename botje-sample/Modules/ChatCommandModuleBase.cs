using Botje.Core;
using Botje.Core.Utils;
using Botje.Messaging;
using Botje.Messaging.Events;
using Botje.Messaging.Models;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Botje.Sample.Modules
{
    public abstract class ChatCommandModuleBase : IBotModule
    {
        protected readonly Func<string, string> _ = (s) => MessageUtils.HtmlEscape(s);

        public enum Source
        {
            Public,
            Private,
            Channel
        }

        protected ILogger Log;

        /// <summary></summary>
        [Inject]
        public IMessagingClient Client { get; set; }

        /// <summary></summary>
        [Inject]
        public ILoggerFactory LoggerFactory { set { Log = value.Create(GetType()); } }

        /// <summary>
        /// </summary>
        public virtual void Shutdown()
        {
            Client.OnPrivateMessage -= Client_OnPrivateMessage;
            Client.OnChannelMessage -= Client_OnChannelMessage;
            Client.OnPublicMessage -= Client_OnPublicMessage;
            Log.Trace($"Shut down {GetType().Name}");
        }

        /// <summary>
        /// </summary>
        public virtual void Startup()
        {
            Log.Trace($"Started {GetType().Name}");
            Client.OnPrivateMessage += Client_OnPrivateMessage;
            Client.OnChannelMessage += Client_OnChannelMessage;
            Client.OnPublicMessage += Client_OnPublicMessage;
        }

        private void Client_OnPublicMessage(object sender, PublicMessageEventArgs e)
        {
            ProcessMessage(Source.Public, e.Message);
        }

        private void Client_OnChannelMessage(object sender, ChannelMessageEventArgs e)
        {
            ProcessMessage(Source.Channel, e.Message);
        }

        private void Client_OnPrivateMessage(object sender, PrivateMessageEventArgs e)
        {
            ProcessMessage(Source.Private, e.Message);
        }

        private void ProcessMessage(Source source, Message message)
        {
            var me = Client.GetMe();
            var firstEntity = message?.Entities?.FirstOrDefault();
            if (null != firstEntity && firstEntity.Type == "bot_command" && firstEntity.Offset == 0)
            {
                string myName = Client.GetMe().Username;
                string command = message.Text.Substring(firstEntity.Offset, firstEntity.Length);
                if (command.Contains("@") && !command.EndsWith($"@{myName}", StringComparison.InvariantCultureIgnoreCase))
                {
                    // not for me
                    Log.Trace($"Got command '{command}' but it is not for me.");
                }
                else
                {
                    command = command.Split("@").First()?.ToLowerInvariant();
                    var args = SplitArgs(message.Text.Substring(firstEntity.Length).Trim()).ToArray();
                    ProcessCommand(source, message, command, args);
                }
            }
        }

        protected static IEnumerable<string> SplitArgs(string argstr)
        {
            List<string> result = new List<string>();

            string token = "";
            bool isOpen = false;

            argstr = argstr?.Trim() ?? string.Empty;
            for (int index = 0; index < argstr.Length; index++)
            {
                char c = argstr[index];
                if (c == '"') // if it's a quote, toggle isopen to either start or stop the mode in which we consume anything
                {
                    isOpen = !isOpen;
                }
                else if (c == '\\') // if escaped just consume the next char without processing it
                {
                    if (index + 1 < argstr.Length) token += argstr[index + 1];
                    index++; // skip
                }
                else if (!isOpen && char.IsWhiteSpace(c)) // whitespace outside quoted expression
                {
                    // ignore
                    if (!string.IsNullOrWhiteSpace(token)) result.Add(token);
                    token = "";
                }
                else // inside quoted expression, or not whitespace
                {
                    token += c;
                }
            }
            if (!string.IsNullOrWhiteSpace(token)) result.Add(token);
            return result;
        }

        /// <summary>
        /// Process a command message with (potentially quoted) arguments on public, private and channel-chats.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="command"></param>
        /// <param name="args"></param>
        public abstract void ProcessCommand(Source source, Message message, string command, string[] args);

    }
}
