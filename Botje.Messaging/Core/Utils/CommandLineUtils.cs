using System.Collections.Generic;

namespace Botje.Core.Utils
{
    /// <summary>
    /// Helper methods for basic commandline parsing.
    /// </summary>
    public static class CommandLineUtils
    {

        /// <summary>
        /// Splits a string into multiple separate arguments.
        /// </summary>
        /// <remarks>
        /// By default, string is split on whitespace. Parts of the string may be enclosed in double quotes to escape spaces in there. Individual characters may also be escaped by prepending them with a backslash character.
        /// </remarks>
        /// <example>
        /// ...
        /// </example>
        /// <param name="argstr">Raw argument string that is to besplit into separate parts.</param>
        /// <returns></returns>
        public static IEnumerable<string> SplitArgs(string argstr)
        {
            List<string> result = new List<string>();

            string token = "";
            bool isOpen = false;

            argstr = argstr?.Trim() ?? string.Empty;
            for (int index = 0; index < argstr.Length; index++)
            {
                char c = argstr[index];
                if (c == '"') // if it's a quote, toggle isopen to either start or stop the mode in which we consume anything
                {
                    isOpen = !isOpen;
                }
                else if (c == '\\') // if escaped just consume the next char without processing it
                {
                    if (index + 1 < argstr.Length) token += argstr[index + 1];
                    index++; // skip
                }
                else if (!isOpen && char.IsWhiteSpace(c)) // whitespace outside quoted expression
                {
                    // ignore
                    if (!string.IsNullOrWhiteSpace(token)) result.Add(token);
                    token = "";
                }
                else // inside quoted expression, or not whitespace
                {
                    token += c;
                }
            }
            if (!string.IsNullOrWhiteSpace(token)) result.Add(token);
            return result;
        }
    }
}
