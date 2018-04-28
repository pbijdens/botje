namespace Botje.Core.Utils
{
    /// <summary>
    /// Utils for working with messages.
    /// </summary>
    public static class MessageUtils
    {
        /// <summary>
        /// Escapes text specifically for using it in Telegram HTML messages.
        /// </summary>
        /// <param name="s">String to be escaped</param>
        /// <returns>Escaped string</returns>
        public static string HtmlEscape(string s) => (s ?? "").Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;");
    }
}
