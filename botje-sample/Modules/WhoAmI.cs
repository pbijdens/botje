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
            Client.SendMessageToChat(conversationID, $"<b>User properties:</b>"
                + $"\n - FirstName = {who.FirstName}"
                + $"\n - ID = {who.ID}"
                + $"\n - IsBot = {who.IsBot}"
                + $"\n - LanguageCode = {who.LanguageCode}"
                + $"\n - LastName = {who.LastName}"
                + $"\n - Username = {who.Username}"
                + $"\n - DisplayName() = {who.DisplayName()}" // calculated
                + $"\n - ShortName() = {who.ShortName()}" // calculated
                + $"\n - UsernameOrName() = {who.UsernameOrName()}" // calculated
                );
        }
    }
}
