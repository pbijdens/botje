using Botje.Core;
using Botje.Core.Commands;
using Botje.Core.Loggers;
using Botje.Core.Utils;
using Botje.DB;
using Botje.Messaging;
using Botje.Messaging.PrivateConversation;
using Botje.Messaging.Telegram;
using Botje.Sample.Modules;
using Botje.Sample.Utils;
using Ninject;
using System;
using System.Linq;
using System.Threading;

namespace Botje.Sample
{
    class Program
    {
        /// <summary>
        /// Arrgs[0] should be the API key
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine($"This program requires the Telegram bot API key as a single argument.");
                return;
            }

            // Initialize globals
            string apiKey = args[0];

            // Create the settings service
            var settings = JsonSettings.FromFile("settings.json");
            TimeUtils.Initialize(settings.Timezones.ToArray());

            // initialize the kernel
            var kernel = new StandardKernel();
            kernel.Bind<ISettingsService>().ToConstant(settings);
            kernel.Bind<ILoggerFactory>().To<ConsoleLoggerFactory>(); // provided by Botje.Core, replace with your favorite logging framework

            // Set up the database
            var database = kernel.Get<Database>();
            database.Setup(settings.DataDirectory);
            kernel.Bind<IDatabase>().ToConstant(database);
            kernel.Bind<IPrivateConversationManager>().To<PrivateConversationManager>().InSingletonScope(); // helper for storing per-user conversation state in the database

            // Set up the messaging client
            CancellationTokenSource source = new CancellationTokenSource();
            TelegramClient client = kernel.Get<ThrottlingTelegramClient>();
            client.Setup(apiKey, source.Token);
            kernel.Bind<IMessagingClient>().ToConstant(client);

            // Register the console commands
            kernel.Bind<IConsoleCommand>().To<PingCommand>().InSingletonScope(); // provided by the core framework
            kernel.Bind<IConsoleCommand>().To<HelpCommand>().InSingletonScope(); // provided by the core framework
            kernel.Bind<IConsoleCommand>().To<LogLevelCommand>().InSingletonScope(); // provided by the core framework
            kernel.Bind<IConsoleCommand>().ToConstant(new ConsoleCommands.ExitCommand { TokenSource = source }).InSingletonScope();
            kernel.Bind<IConsoleCommand>().To<ConsoleCommands.MeCommand>().InSingletonScope();

            // Register the bot modules
            kernel.Bind<IBotModule>().To<WhereAmI>().InSingletonScope();
            kernel.Bind<IBotModule>().To<WhoAmI>().InSingletonScope();

            // Register API handlers
            var modules = kernel.GetAll<IBotModule>().ToList();

            // Boot
            modules.ForEach(m => m.Startup());
            client.Start();

            // Run the console loop in the background
            var consoleLoop = kernel.Get<ConsoleLoop>();
            consoleLoop.Run(source.Token);

            // Shut down the modules
            modules.ForEach(m => m.Shutdown());

            // Say goodbye. It's the decent thing to do.
            Console.WriteLine("Bot was terminated. Have a nice day.");
        }
    }
}
