namespace Botje.Messaging.Models
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class TelegramAPIObjectBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.GetType().FullName + "=" + Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
