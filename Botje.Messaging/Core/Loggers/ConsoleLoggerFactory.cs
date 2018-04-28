using System;

namespace Botje.Core.Loggers
{
    /// <summary>
    /// Factory for the console logger.
    /// </summary>
    public class ConsoleLoggerFactory : ILoggerFactory
    {
        /// <summary>
        /// Constructs a console logger object.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public ILogger Create(Type t)
        {
            return new ConsoleLogger() { Class = t.Name };
        }
    }
}
