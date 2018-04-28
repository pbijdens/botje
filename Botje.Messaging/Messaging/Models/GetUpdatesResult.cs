using RestSharp.Deserializers;

namespace Botje.Messaging.Models
{
    /// <summary>
    /// https://core.telegram.org/bots/api#getting-updates
    /// unnamed object that acts as the reult of the getUpdates call, webhook, etc.
    /// </summary>
    public class GetUpdatesResult : TelegramAPIObjectBase
    {
        [DeserializeAs(Name = "update_id")]
        public long UpdateID { get; set; }

        [DeserializeAs(Name = "message")]
        public Message Message { get; set; }

        [DeserializeAs(Name = "edited_message")]
        public Message EditedMessage { get; set; }

        [DeserializeAs(Name = "channel_post")]
        public Message ChannelPost { get; set; }

        [DeserializeAs(Name = "edited_channel_post")]
        public Message EditedChannelPost { get; set; }

        [DeserializeAs(Name = "inline_query")]
        public InlineQuery InlineQuery { get; set; }

        [DeserializeAs(Name = "chosen_inline_result")]
        public ChosenInlineResult ChosenInlineResult { get; set; }

        [DeserializeAs(Name = "callback_query")]
        public CallbackQuery CallbackQuery { get; set; }

        public UpdateType GetUpdateType()
        {
            if (null != Message) return UpdateType.Message;
            if (null != EditedMessage) return UpdateType.EditedMessage;
            if (null != ChannelPost) return UpdateType.ChannelPost;
            if (null != EditedChannelPost) return UpdateType.EditedChannelPost;
            if (null != InlineQuery) return UpdateType.InlineQuery;
            if (null != ChosenInlineResult) return UpdateType.ChosenInlineResult;
            if (null != CallbackQuery) return UpdateType.CallbackQuery;
            return UpdateType.Unsuported;
        }

        // Not supported (bot API does not support any of the invoicing - shipping workflow yet).
        // shipping_query	ShippingQuery	Optional. New incoming shipping query. Only for invoices with flexible price
        // pre_checkout_query	PreCheckoutQuery	Optional. New incoming pre-checkout query. Contains full information about checkout
    }
}