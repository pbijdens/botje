using System.Collections.Generic;

namespace Botje.Messaging.Models
{
    /// <summary>
    /// https://core.telegram.org/bots/api#inlinekeyboardmarkup
    /// This object represents an inline keyboard that appears right next to the message it belongs to.
    /// </summary>
    public class InlineKeyboardMarkup : TelegramAPIObjectBase
    {
        public List<List<InlineKeyboardButton>> inline_keyboard { get; set; }
    }
}
