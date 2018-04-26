using Models = Botje.Messaging.Models;

namespace Botje.Sample.Modules
{
    /// <summary>
    /// Responds to the /DeleteMessage command. Informs the user about the chat they are in.
    /// </summary>
    public class DeleteMessage : ChatCommandModuleBase
    {
        public override void ProcessCommand(Source source, Models.Message message, string command, string[] args)
        {
            var id = message.Chat.ID;

            switch (command)
            {
                case "/deletemessage":
                    CmdDeleteMessage(message, args);
                    break;
            }
        }

        private void CmdDeleteMessage(Models.Message message, string[] args)
        {
            if (args.Length != 1)
            {
                Client.SendMessageToChat(message.Chat.ID, $"@{_(message.From.UsernameOrName())} use /deletemessage &lt;messageid&gt;", "HTML", null, null, message.MessageID);
                return;
            }
            if (!long.TryParse(args[0], out long messageID))
            {
                Client.SendMessageToChat(message.Chat.ID, $"@{_(message.From.UsernameOrName())} use /deletemessage &lt;messageid&gt; where message ID should be a valid message ID number, not '{_(args[0])}'...", "HTML", null, null, message.MessageID);
                return;
            }
            bool success = Client.DeleteMessage(message.Chat.ID, messageID);
            Client.SendMessageToChat(message.Chat.ID, $"@{_(message.From.UsernameOrName())} deleting message '{messageID}' yielded result {success}...", "HTML", null, null, message.MessageID);
        }
    }
}
