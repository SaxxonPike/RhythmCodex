using System;
using System.Collections.Generic;

namespace RhythmCodex.Cli
{
    /// <summary>
    /// Contains information about a command that belongs to a module.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Describes the command's functionality.
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Execute the function with the specified command line parameters.
        /// </summary>
        Action<IDictionary<string, string[]>> Execute { get; }
        
        /// <summary>
        /// Name of the command the user can refer to this command with.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Available parameters for this command.
        /// </summary>
        IEnumerable<ICommandParameter> Parameters { get; }
    }
}