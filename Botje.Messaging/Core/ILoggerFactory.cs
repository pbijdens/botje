using System;

namespace Botje.Core
{
    /// <summary>
    /// Interface for classes that create logger objects.
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Create a new logger for this type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        ILogger Create(Type t);
    }
}
