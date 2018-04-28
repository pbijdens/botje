using Botje.DB;
using System;

namespace Botje.Messaging.Telegram
{
    /// <summary>
    /// Represents the state of the telegram bot itself. Actualy a persistent singleton object in the DB.
    /// </summary>
    public class TelegramBotState : IAtom
    {
        public Guid UniqueID { get; set; }

        public long LastProcessedUpdateID { get; set; }
    }
}