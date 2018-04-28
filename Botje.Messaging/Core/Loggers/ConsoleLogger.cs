using Botje.Core.Utils;
using System;

namespace Botje.Core.Loggers
{
    /// <summary>
    /// Logs to the debug console.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Intended to hold the name of the class for which the console logger was created.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="text"></param>
        public void Error(string text)
        {
            if (ConsoleLogLevel > LogLevel.Error) return;
            Console.WriteLine($"{DateTime.Now} ERROR ({Class}): {text}");
        }

        /// <summary>
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="text"></param>
        public void Error(Exception ex, string text)
        {
            if (ConsoleLogLevel > LogLevel.Error) return;
            Console.WriteLine($"{DateTime.Now} ERROR ({Class}): {text}\r\n{ExceptionUtils.AsString(ex)}");
        }

        /// <summary>
        /// </summary>
        /// <param name="text"></param>
        public void Info(string text)
        {
            if (ConsoleLogLevel > LogLevel.Info) return;
            Console.WriteLine($"{DateTime.Now} INFO ({Class}): {text}");
        }

        /// <summary>
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="text"></param>
        public void Info(Exception ex, string text)
        {
            if (ConsoleLogLevel > LogLevel.Info) return;
            Console.WriteLine($"{DateTime.Now} INFO ({Class}): {text}\r\n{ExceptionUtils.AsString(ex)}");
        }

        /// <summary>
        /// </summary>
        /// <param name="text"></param>
        public void Trace(string text)
        {
            if (ConsoleLogLevel > LogLevel.Trace) return;
            Console.WriteLine($"{DateTime.Now} TRACE ({Class}): {text}");
        }

        /// <summary>
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="text"></param>
        public void Trace(Exception ex, string text)
        {
            if (ConsoleLogLevel > LogLevel.Trace) return;
            Console.WriteLine($"{DateTime.Now} TRACE ({Class}): {text}\r\n{ExceptionUtils.AsString(ex)}");
        }

        /// <summary>
        /// </summary>
        /// <param name="text"></param>
        public void Warn(string text)
        {
            if (ConsoleLogLevel > LogLevel.Warn) return;
            Console.WriteLine($"{DateTime.Now} WARN ({Class}): {text}");
        }

        /// <summary>
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="text"></param>
        public void Warn(Exception ex, string text)
        {
            if (ConsoleLogLevel > LogLevel.Warn) return;
            Console.WriteLine($"{DateTime.Now} WARN ({Class}): {text}\r\n{ExceptionUtils.AsString(ex)}");
        }

        /// <summary>
        /// </summary>
        public static LogLevel ConsoleLogLevel = LogLevel.Info;

        /// <summary>
        /// </summary>
        /// <param name="level"></param>
        public void SetLevel(LogLevel level)
        {
            ConsoleLogLevel = level;
        }
    }
}
