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
            Client.SendMessageToChat(chatID, $"<b>Chat properties:</b> "
                + $"\n - ID = {chat.ID}"
                + $"\n - Description = {chat.Description}"
                + $"\n - Username = {chat.Username}"
                + $"\n - FirstName = {chat.FirstName}"
                + $"\n - LastName = {chat.LastName}"
                + $"\n - InviteLink = {chat.InviteLink}"
                + $"\n - PinnedMessage.ID = {chat.PinnedMessage?.MessageID}"
                + $"\n - PinnedMessage.Text = {chat.PinnedMessage?.Text}"
                + $"\n - Title = {chat.Title}"
                + $"\n - AllMembersAreAdministrators = {chat.AllMembersAreAdministrators}"
                + $"\n - Type = {chat.Type}"
                );
        }
    }
}
