using System;
using System.Collections.Generic;
using ClientCommon;
using RhythmCodex.Cli.Orchestration.Infrastructure;

namespace RhythmCodex.Cli;

/// <inheritdoc />
public class Command : ICommand
{
    /// <inheritdoc />
    public required string Name { get; init; }

    /// <inheritdoc />
    public IEnumerable<ICommandParameter> Parameters { get; init; } = 
        Array.Empty<ICommandParameter>();

    /// <inheritdoc />
    public required string Description { get; init; }

    /// <inheritdoc />
    public Func<Args, ITask> TaskFactory { get; init; } = 
        _ => throw new Exception("Execute is not defined.");
}