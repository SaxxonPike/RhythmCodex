using System;
using System.Collections.Generic;
using ClientCommon;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;

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
        /// Gets a delegate that can be used to create tasks that execute this command.
        /// </summary>
        Func<Args, ITask> TaskFactory { get; }
        
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