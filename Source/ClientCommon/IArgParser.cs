using System.Collections.Generic;
using JetBrains.Annotations;

namespace ClientCommon;

/// <summary>
/// Parses command line arguments into key-value pairs.
/// </summary>
[PublicAPI]
public interface IArgParser
{
    /// <summary>
    /// Parse the specified command line arguments into key-value pairs.
    /// </summary>
    Args Parse(IEnumerable<string> args);
}