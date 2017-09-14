using System.Collections.Generic;

namespace RhythmCodex.Cli.Helpers
{
    public interface IArgParser
    {
        IDictionary<string, string[]> Parse(IEnumerable<string> args);
    }
}