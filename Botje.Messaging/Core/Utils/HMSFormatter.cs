using System;
using System.Collections.Generic;

namespace Botje.Core.Utils
{
    /// <summary>
    /// Custom formatter for DateTime objects, allowing more friendly text output. 
    /// Initialized by default for Dutch, but feel free to initialize it any way you want.
    /// </summary>
    public class HMSFormatter : ICustomFormatter, IFormatProvider
    {
        // List of Formats, with a P customformat for pluralization
        public static Dictionary<string, string> TimeFormats = new Dictionary<string, string> {
                {"S", "{0:P:seconden:seconde}"},
                {"M", "{0:P:minuten:minuut}"},
                {"H","{0:P:uur:uur}"},
                {"D", "{0:P:dagen:dag}"}
            };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            return String.Format(new PluralFormatter(), TimeFormats[format], arg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }
    }
}
