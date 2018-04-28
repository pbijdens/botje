using RestSharp.Deserializers;

namespace Botje.Messaging.Models
{
    /// <summary>
    /// https://core.telegram.org/bots/api#document
    /// </summary>
    public class Document : TelegramAPIObjectBase
    {
        /// <summary>
        /// Use GetFile()
        /// </summary>
        [DeserializeAs(Name = "file_id")] public string FileID { get; set; }
        [DeserializeAs(Name = "thumb")] public PhotoSize Thumb { get; set; }
        [DeserializeAs(Name = "file_name")] public string Filename { get; set; }
        [DeserializeAs(Name = "title")] public string Title { get; set; }
        [DeserializeAs(Name = "mime_type")] public string MimeType { get; set; }
        [DeserializeAs(Name = "file_size")] public long FileSize { get; set; }
    }
}