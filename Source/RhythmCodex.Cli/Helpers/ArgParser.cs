using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli.Helpers
{
    [Service]
    public class ArgParser : IArgParser
    {
        public IDictionary<string, string[]> Parse(IEnumerable<string> args)
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
        
    }
}