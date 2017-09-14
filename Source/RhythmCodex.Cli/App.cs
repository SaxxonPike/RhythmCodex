using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Cli.Helpers;

namespace RhythmCodex.Cli
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class App : IApp
    {
        private readonly TextWriter _logger;
        private readonly IEnumerable<ICliModule> _modules;
        private readonly IArgParser _argParser;

        public App(TextWriter logger, IEnumerable<ICliModule> modules, IArgParser argParser)
        {
            _logger = logger;
            _modules = modules;
            _argParser = argParser;
        }

        private static string AppName => "RhythmCodex";

        private static bool InvariantStringMatch(string a, string b)
        {
            return string.Equals(a, b, StringComparison.CurrentCultureIgnoreCase);
        }

        public void Run(string[] args)
        {
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

            command.Execute(_argParser.Parse(args.Skip(2)));
        }


        private void OutputCommandList(ICliModule module)
        {
            _logger.WriteLine($"{AppName}");
            _logger.WriteLine($"Available commands for {module.Name}:");
            _logger.WriteLine();
            
            foreach (var command in module.Commands.OrderBy(c => c.Name))
            {
                _logger.WriteLine($"{command.Name.PadRight(20)}{command.Description}");
            }
            
            _logger.WriteLine();
            _logger.WriteLine("To learn more about a command:");
            _logger.WriteLine($"{AppName} {module.Name.ToLower()} <command>");
        }

        private void OutputModuleList()
        {
            _logger.WriteLine($"{AppName}");
            _logger.WriteLine("Available modules:");
            _logger.WriteLine();

            foreach (var module in _modules)
            {
                _logger.WriteLine($"{module.Name.PadRight(20)}{module.Description}");
            }
            
            _logger.WriteLine();
            _logger.WriteLine("To obtain a list of commands for a module:");
            _logger.WriteLine($"{AppName} <module>");
        }
    }
}
