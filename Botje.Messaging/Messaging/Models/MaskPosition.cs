using RestSharp.Deserializers;

namespace Botje.Messaging.Models
{
    /// <summary>
    /// https://core.telegram.org/bots/api#maskposition
    /// </summary>
    public class MaskPosition : TelegramAPIObjectBase
    {
        [DeserializeAs(Name = "point")] public string Point { get; set; }
        [DeserializeAs(Name = "x_shift")] public double XShift { get; set; }
        [DeserializeAs(Name = "y_shift")] public double YShift { get; set; }
        [DeserializeAs(Name = "scale")] public double Scale { get; set; }
    }
}