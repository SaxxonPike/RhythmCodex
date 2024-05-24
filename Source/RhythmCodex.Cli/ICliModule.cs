using System.Collections.Generic;

namespace RhythmCodex.Cli;

/// <summary>
/// Contains information about a module.
/// </summary>
public interface ICliModule
{
    /// <summary>
    /// Name the user can refer to this module with.
    /// </summary>
    string Name { get; }
        
    /// <summary>
    /// Describes this module's use.
    /// </summary>
    string Description { get; }
        
    /// <summary>
    /// Contains the available commands for this module.
    /// </summary>
    IEnumerable<ICommand> Commands { get; }
}