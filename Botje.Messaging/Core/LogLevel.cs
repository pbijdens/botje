namespace Botje.Core
{
    /// <summary>
    /// Log levels supported by this tiny logging framework.
    /// </summary>
    public enum LogLevel : int
    {
        All = 0,
        Trace,
        Info,
        Warn,
        Error,
        None = int.MaxValue
    }
}
