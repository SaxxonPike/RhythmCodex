using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RhythmCodex.Cli.Modules;
using RhythmCodex.Ioc;

namespace RhythmCodex.Cli
{
    public class App
    {
        private readonly TextWriter _logger;
        private IEnumerable<ICliModule> _modules;

        public App(TextWriter logger)
        {
            _logger = logger;
        }

        private string AppName
        {
            get
            {
                var name = AppDomain.CurrentDomain.FriendlyName.Split('.');
                return string.Join(".", name.Take(name.Length - 1));
            }
        }

        private string AppVersion
        {
            get
            {
                var ver = GetType().Assembly.GetName().Version;
                return $"{ver.Major}.{ver.Minor}.{ver.Revision}.{ver.Build}";
            }
        }

        private bool InvariantStringMatch(string a, string b)
        {
            return string.Equals(a, b, StringComparison.CurrentCultureIgnoreCase);
        }

        public void Run(string[] args, IEnumerable<ICliModule> modules)
        {
            _modules = modules;
            
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

            command.Execute(ParseArgs(args.Skip(2)));
        }

        private IDictionary<string, string[]> ParseArgs(IEnumerable<string> args)
        {
            var result = new Dictionary<string, List<string>>();
            var current = string.Empty;
            var optsEnabled = true;
            
            foreach (var arg in args)
            {
                var addValue = true;
                
                if (arg == "--")
                {
                    current = string.Empty;
                    optsEnabled = false;
                    continue;
                }

                if (optsEnabled)
                {
                    if (arg == "-")
                        continue;
                    
                    if (arg.StartsWith("-"))
                    {
                        current = arg.Substring(1);
                        addValue = false;
                    }
                }
                
                if (!result.ContainsKey(current))
                    result[current] = new List<string>();
                
                if (addValue)
                    result[current].Add(arg);
            }

            return result.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());
        }

        private void OutputCommandList(ICliModule module)
        {
            _logger.WriteLine($"{AppName} v{AppVersion}");
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
            _logger.WriteLine($"{AppName} v{AppVersion}");
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
