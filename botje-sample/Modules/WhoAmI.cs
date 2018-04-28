using Models = Botje.Messaging.Models;

namespace Botje.Sample.Modules
{
    /// <summary>
    /// Responds to the /Whoami command. Informs the user about the chat they are in.
    /// </summary>
    public class WhoAmI : ChatCommandModuleBase
    {
        public override void ProcessCommand(Source source, Models.Message message, string command, string[] args)
        {
            switch (command)
            {
                case "/whoami":
                    CmdWhoAmI(message.Chat.ID, message.From);
                    break;
            }
        }

        private void CmdWhoAmI(long conversationID, Models.User who)
        {
            Client.SendMessageToChat(conversationID, $"<b>User:</b> " + _(who.ToString()));
        }
    }
}
