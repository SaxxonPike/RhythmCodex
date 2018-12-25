using System.Collections.Generic;

namespace RhythmCodex.Cli.Helpers
{
    /// <summary>
    /// Parses command line arguments into key-value pairs.
    /// </summary>
    public interface IArgParser
    {
        /// <summary>
        /// Parse the specified command line arguments into key-value pairs.
        /// </summary>
        Args Parse(IEnumerable<string> args);
    }
}