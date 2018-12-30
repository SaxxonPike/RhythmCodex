using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli.Helpers
{
    public sealed class Args
    {
        public Args(IDictionary<string, string[]> options)
        {
            var opts = options.ToDictionary(kv => kv.Key, kv => kv.Value);
            Options = opts;

            OutputPath = opts.ContainsKey("o")
                ? opts["o"].Last()
                : null;
            opts.Remove("o");
            
            InputFiles = opts.ContainsKey(string.Empty)
                ? opts[string.Empty]
                : new string[0];
            opts.Remove(string.Empty);

            RecursiveInputFiles = opts.ContainsKey("+r");
            opts.Remove("+r");

            if (opts.ContainsKey("log"))
            {
                switch (opts["log"].FirstOrDefault()?.ToLowerInvariant())
                {
                    case "debug":
                        LogLevel = LoggerVerbosityLevel.Debug;
                        break;
                    case "info":
                        LogLevel = LoggerVerbosityLevel.Info;
                        break;
                    case "warning":
                        LogLevel = LoggerVerbosityLevel.Warning;
                        break;
                    case "error":
                        LogLevel = LoggerVerbosityLevel.Error;
                        break;
                }            
            }
        }
        
        public IReadOnlyDictionary<string, string[]> Options { get; }
        public string OutputPath { get; }
        public IReadOnlyList<string> InputFiles { get; }
        public bool RecursiveInputFiles { get; }
        public LoggerVerbosityLevel LogLevel { get; } = LoggerVerbosityLevel.Info;
    }
}