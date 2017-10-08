using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac.Core;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli
{
    /// <inheritdoc />
    public class App : IApp
    {
        private readonly IArgParser _argParser;
        private readonly ILoggerConfigurationSource _loggerConfigurationSource;
        private readonly TextWriter _console;
        private readonly IEnumerable<ICliModule> _modules;

        /// <summary>
        /// Create an instance of the main application container.
        /// </summary>
        public App(
            TextWriter console,
            IEnumerable<ICliModule> modules,
            IArgParser argParser,
            ILoggerConfigurationSource loggerConfigurationSource)
        {
            _console = console;
            _modules = modules;
            _argParser = argParser;
            _loggerConfigurationSource = loggerConfigurationSource;
        }

        private string AppName => "RhythmCodex";

        private string AppVersion =>
            typeof(App)
                .GetTypeInfo()
                .Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;

        /// <inheritdoc />
        public void Run(string[] args)
        {
            _console.WriteLine($"{AppName} {AppVersion}");

            if (args.Length < 1)
            {
                OutputModuleList();
                return;
            }

            var module = _modules.FirstOrDefault(m => InvariantStringMatch(args[0], m.Name));

            if (module == null)
            {
                OutputModuleList();
                return;
            }

            if (args.Length < 2)
            {
                OutputCommandList(module);
                return;
            }

            var command = module.Commands.FirstOrDefault(c => InvariantStringMatch(args[1], c.Name));
            if (command == null)
            {
                OutputCommandList(module);
                return;
            }

            var parameters = _argParser.Parse(args.Skip(2));
            var moduleParameters = ProcessSpecialArgs(parameters);

            if (!moduleParameters.Any())
            {
                OutputParameterList(module, command);
                return;
            }

            _console.WriteLine($"Executing {module.Name} {command.Name}.");
            command.Execute(moduleParameters);
            _console.WriteLine($"Task complete.");
        }

        /// <summary>
        /// Obtain special arguments that do not pertain to any one module.
        /// </summary>
        private IDictionary<string, string[]> ProcessSpecialArgs(IDictionary<string, string[]> args)
        {
            var specialArgs = args.Where(a => a.Key.StartsWith("-"));
            var filteredArgs = args.Where(a => !a.Key.StartsWith("-"));

            foreach (var arg in specialArgs)
            {
                switch (arg.Key.ToLowerInvariant().Substring(1))
                {
                    case "log":
                        SetLogLevel(arg.Value.LastOrDefault());
                        break;
                }
            }

            return filteredArgs.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        /// <summary>
        /// Set the logger level, overriding the currently configured level.
        /// </summary>
        private void SetLogLevel(string logLevel)
        {
            switch ((logLevel ?? string.Empty).ToLowerInvariant())
            {
                case "debug":
                    _loggerConfigurationSource.VerbosityLevel = LoggerVerbosityLevel.Debug;
                    break;
                case "info":
                    _loggerConfigurationSource.VerbosityLevel = LoggerVerbosityLevel.Info;
                    break;
                case "warning":
                    _loggerConfigurationSource.VerbosityLevel = LoggerVerbosityLevel.Warning;
                    break;
                case "error":
                    _loggerConfigurationSource.VerbosityLevel = LoggerVerbosityLevel.Error;
                    break;
            }
            _console.WriteLine($"Using {_loggerConfigurationSource.VerbosityLevel} log level.");
        }

        /// <summary>
        /// Compare two strings, ignoring case.
        /// </summary>
        private static bool InvariantStringMatch(string a, string b)
        {
            return string.Equals(a, b, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Inform the user of parameters for a particular module's command.
        /// </summary>
        private void OutputParameterList(ICliModule module, ICommand command)
        {
            _console.WriteLine($"Available parameters for {module.Name} {command.Name}:");
            _console.WriteLine();

            foreach (var parameter in command.Parameters)
                _console.WriteLine($"{parameter.Name.PadRight(20)}{parameter.Description}");

            _console.WriteLine();
            _console.WriteLine("Executing this command with any parameters will perform the action.");
        }

        /// <summary>
        /// Inform the user of commands available to a particular module.
        /// </summary>
        private void OutputCommandList(ICliModule module)
        {
            _console.WriteLine($"Available commands for {module.Name}:");
            _console.WriteLine();

            foreach (var command in module.Commands.OrderBy(c => c.Name))
                _console.WriteLine($"{command.Name.PadRight(20)}{command.Description}");

            _console.WriteLine();
            _console.WriteLine("To learn more about a command:");
            _console.WriteLine($"{AppName} {module.Name.ToLower()} <command>");
        }

        /// <summary>
        /// Inform the user of the available modules.
        /// </summary>
        private void OutputModuleList()
        {
            _console.WriteLine("Available modules:");
            _console.WriteLine();

            foreach (var module in _modules)
                _console.WriteLine($"{module.Name.PadRight(20)}{module.Description}");

            _console.WriteLine();
            _console.WriteLine("To obtain a list of commands for a module:");
            _console.WriteLine($"{AppName} <module>");
        }
    }
}