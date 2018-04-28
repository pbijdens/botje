using RestSharp.Deserializers;

namespace Botje.Messaging.Models
{
    /// <summary>
    /// https://core.telegram.org/bots/api#location
    /// </summary>
    public class Location : TelegramAPIObjectBase
    {
        // longitude Float   Longitude as defined by sender
        [DeserializeAs(Name = "longitude")] public double Longitude { get; set; }

        // latitude    Float Latitude as defined by sender
        [DeserializeAs(Name = "latitude")] public double Latitude { get; set; }
    }
}