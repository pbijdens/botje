namespace Botje.Core.Commands
{
    /// <summary>
    /// Interface for console commands, allowing them to be injected into the console loop.
    /// </summary>
    public interface IConsoleCommand
    {
        /// <summary>
        /// Command description.
        /// </summary>
        CommandInfo Info { get; }

        /// <summary>
        /// Invoked when the software is started. Can be used to perform additional setup tasks.
        /// </summary>
        /// <param name="logger"></param>
        void OnStart(ILogger logger);

        /// <summary>
        /// Invoked when input arrives for this command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        bool OnInput(string command, string[] args);
    }
}
