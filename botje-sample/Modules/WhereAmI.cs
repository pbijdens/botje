using Models = Botje.Messaging.Models;

namespace Botje.Sample.Modules
{
    /// <summary>
    /// Responds to the /whereami command. Informs the user about the chat they are in.
    /// </summary>
    public class WhereAmI : ChatCommandModuleBase
    {
        public override void ProcessCommand(Source source, Models.Message message, string command, string[] args)
        {
            switch (command)
            {
                case "/whereami":
                    CmdWhereAmI(message.Chat.ID);
                    break;
            }
        }

        private void CmdWhereAmI(long chatID)
        {
            Models.Chat chat = Client.GetChat(chatID);
            Client.SendMessageToChat(chatID, $"<b>Chat:</b> " + _(chat.ToString()));

        }
    }
}
