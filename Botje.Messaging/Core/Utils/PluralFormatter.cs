using System;

namespace Botje.Core.Utils
{
    /// <summary>
    /// Helper class for formatting plurals. Blatently copied from stackoverflow.
    /// </summary>
    public class PluralFormatter : ICustomFormatter, IFormatProvider
    {
        /// <summary>
        /// Adds a special 'P' string format support for properly formatting plurals. The format string is then {0:P:pluralform:singularform}.
        /// </summary>
        /// <example>
        /// &quot;{0} {0:P:days:day}&quot; will translate to &quot;1 day&quot;, &quot;0 days&quot; and &quot;42 days&quot;
        /// </example>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg != null)
            {
                var parts = format.Split(':'); // ["P", "Plural", "Singular"]

                if (parts[0] == "P") // correct format?
                {
                    // which index postion to use
                    int partIndex = (arg.ToString() == "1") ? 2 : 1;
                    // pick string (safe guard for array bounds) and format
                    return String.Format("{0} {1}", arg, (parts.Length > partIndex ? parts[partIndex] : ""));
                }
            }
            return String.Format(format, arg);
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
