using System;
using Models = Botje.Messaging.Models;

namespace Botje.Sample.Modules
{
    /// <summary>
    /// Responds to the /Fail command. Informs the user about the chat they are in.
    /// </summary>
    public class Fail : ChatCommandModuleBase
    {
        public override void ProcessCommand(Source source, Models.Message message, string command, string[] args)
        {
            switch (command)
            {
                case "/fail":
                    throw new NullReferenceException($"Failing intentionally!");
            }
        }
    }
}
