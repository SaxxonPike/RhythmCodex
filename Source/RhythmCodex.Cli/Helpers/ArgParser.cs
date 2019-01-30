using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Helpers
{
    [Service]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ArgParser : IArgParser
    {
        /// <inheritdoc />
        public Args Parse(IEnumerable<string> args)
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

                    if (arg.StartsWith("+"))
                    {
                        result[arg] = new List<string>();
                        continue;
                    }
                    else if (arg.StartsWith("-"))
                    {
                        current = arg.Substring(1);
                        addValue = false;
                    }
                }

                if (!result.ContainsKey(current))
                    result[current] = new List<string>();

                if (!addValue) 
                    continue;
                
                result[current].Add(arg);
                current = string.Empty;
            }

            return new Args(result.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray())); 
        }
    }
}