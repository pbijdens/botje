using RestSharp.Deserializers;

namespace Botje.Messaging.Models
{
    /// <summary>
    /// https://core.telegram.org/bots/api#photosize
    /// </summary>
    public class PhotoSize : TelegramAPIObjectBase
    {
        [DeserializeAs(Name = "file_id")] public string FileID { get; set; }
        [DeserializeAs(Name = "file_size")] public long FileSize { get; set; }
        [DeserializeAs(Name = "width")] public long Width { get; set; }
        [DeserializeAs(Name = "height")] public long Height { get; set; }
    }
}