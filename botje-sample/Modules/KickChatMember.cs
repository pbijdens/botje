using Botje.Core.Utils;
using System;
using Models = Botje.Messaging.Models;

namespace Botje.Sample.Modules
{
    /// <summary>
    /// Responds to the /KickChatMember command. Informs the user about the chat they are in.
    /// </summary>
    public class KickChatMember : ChatCommandModuleBase
    {
        public override void ProcessCommand(Source source, Models.Message message, string command, string[] args)
        {
            switch (command)
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
