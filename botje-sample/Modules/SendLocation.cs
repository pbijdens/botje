using Botje.Messaging.Models;

namespace Botje.Sample.Modules
{
    public class SendLocation : ChatCommandModuleBase
    {
        public override void ProcessCommand(Source source, Message message, string command, string[] args)
        {
            switch (command)
            {
                case "/sendlocation":
                    DoSendLocation(message, args);
                    break;

            }
        }

        private void DoSendLocation(Message message, string[] args)
        {
            if (args.Length != 2)
            {
                Client.SendMessageToChat(message.Chat.ID, $"Usage: /sendlocation &lt;latitude&gt; &lt;longitude&gt;");
                return;
            }

            double.TryParse(args[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double latitude);
            double.TryParse(args[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double longitude);

            var result = Client.SendLocation(message.Chat.ID, latitude, longitude);
            Client.SendMessageToChat(message.Chat.ID, "<b>Result:</b> " + _($"{result}"));
        }
    }
}
