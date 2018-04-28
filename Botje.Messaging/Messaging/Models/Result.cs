using RestSharp.Deserializers;

namespace Botje.Messaging.Models
{
    /// <summary>
    /// Telegram API result class. All results conform to this interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        [DeserializeAs(Name = "ok")]
        public bool OK { get; set; }

        [DeserializeAs(Name = "description")]
        public string Description { get; set; }

        [DeserializeAs(Name = "error_code")]
        public int ErrorCode { get; set; }

        [DeserializeAs(Name = "result")]
        public T Data { get; set; }
    }

    /// <summary>
    /// Telegram API result class without result type. All void results conform to this interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result
    {
        [DeserializeAs(Name = "ok")]
        public bool OK { get; set; }

        [DeserializeAs(Name = "description")]
        public string Description { get; set; }

        [DeserializeAs(Name = "error_code")]
        public int ErrorCode { get; set; }
    }
}