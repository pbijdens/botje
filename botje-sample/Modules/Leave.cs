using Botje.Messaging.Models;

namespace Botje.Sample.Modules
{
    public class Leave : ChatCommandModuleBase
    {
        public override void ProcessCommand(Source source, Message message, string command, string[] args)
        {
            switch (command)
            {
                case "/leave":
                    DoLeave(message, args);
                    break;

            }
        }

        private void DoLeave(Message message, string[] args)
        {
            var result = Client.LeaveChat(message.Chat.ID);
        }
    }
}
