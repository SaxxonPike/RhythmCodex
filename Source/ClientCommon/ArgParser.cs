using System.Collections.Generic;
using System.Linq;
using RhythmCodex.IoC;

namespace ClientCommon;

[Service]

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

                if (arg.StartsWith('+'))
                {
                    result[arg] = [];
                    continue;
                }

                if (arg.StartsWith('-'))
                {
                    current = arg[1..];
                    addValue = false;
                }
            }

            if (!result.ContainsKey(current))
                result[current] = [];

            if (!addValue)
                continue;

            result[current].Add(arg);
            current = string.Empty;
        }

        return new Args(result.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray()));
    }
}