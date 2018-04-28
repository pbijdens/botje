using Botje.Messaging.Models;
using System.IO;

namespace Botje.Sample.Modules
{
    public class SendMedia : ChatCommandModuleBase
    {
        public override void ProcessCommand(Source source, Message message, string command, string[] args)
        {
            switch (command)
            {
                case "/senddocument":
                    DoSendDocument(message, args);
                    break;

                case "/sendphoto":
                    DoSendPhoto(message, args);
                    break;

                case "/sendaudio":
                    DoSendAudio(message, args);
                    break;
            }
        }

        private void DoSendDocument(Message message, string[] args)
        {
            if (args.Length == 0)
            {
                Client.SendMessageToChat(message.Chat.ID, $"Usage: /senddocument &lt;local filename on bot fs&gt;");
                return;
            }

            FileStream fs = new FileStream(args[0], FileMode.Open, FileAccess.Read);
            if (fs.Length > 50 * 1024 * 1024)
            {
                Client.SendMessageToChat(message.Chat.ID, $"Maximum supported file size is 50MB.");
                return;
            }
            var result = Client.SendDocument(message.Chat.ID, fs, Path.GetFileName(args[0]), null, fs.Length, "This is a caption");
            Client.SendMessageToChat(message.Chat.ID, "<b>Result:</b> " + _($"{result}"));
        }

        private void DoSendPhoto(Message message, string[] args)
        {
            if (args.Length == 0)
            {
                Client.SendMessageToChat(message.Chat.ID, $"Usage: /sendphoto &lt;local filename on bot fs&gt;");
                return;
            }

            FileStream fs = new FileStream(args[0], FileMode.Open, FileAccess.Read);
            if (fs.Length > 50 * 1024 * 1024)
            {
                Client.SendMessageToChat(message.Chat.ID, $"Maximum supported file size is 50MB.");
                return;
            }
            var result = Client.SendPhoto(message.Chat.ID, fs, Path.GetFileName(args[0]), null, fs.Length, "This is a caption");
            Client.SendMessageToChat(message.Chat.ID, "<b>Result:</b> " + _($"{result}"));
        }

        private void DoSendAudio(Message message, string[] args)
        {
            if (args.Length == 0)
            {
                Client.SendMessageToChat(message.Chat.ID, $"Usage: /sendaudio &lt;local filename on bot fs&gt;");
                return;
            }

            FileStream fs = new FileStream(args[0], FileMode.Open, FileAccess.Read);
            if (fs.Length > 50 * 1024 * 1024)
            {
                Client.SendMessageToChat(message.Chat.ID, $"Maximum supported file size is 50MB.");
                return;
            }
            var result = Client.SendAudio(message.Chat.ID, fs, Path.GetFileName(args[0]), null, fs.Length, 6, "media performer", "title of the media", "This is a caption");
            Client.SendMessageToChat(message.Chat.ID, "<b>Result:</b> " + _($"{result}"));
        }
    }
}
