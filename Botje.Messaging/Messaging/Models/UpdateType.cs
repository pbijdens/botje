namespace Botje.Messaging.Models
{
    /// <summary>
    /// Convenience class representing a type of update.
    /// </summary>
    public enum UpdateType
    {
        Message,
        EditedMessage,
        ChannelPost,
        EditedChannelPost,
        InlineQuery,
        ChosenInlineResult,
        CallbackQuery,
        Unsuported
    }
}
