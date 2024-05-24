using System.Collections.Generic;
using ClientCommon;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Modules;

/// <summary>
/// A module which operates with the SSQ and other associated file formats.
/// </summary>
[Service]
public class SsqCliModule : ICliModule
{
    private readonly ITaskFactory _taskFactory;

    /// <summary>
    /// Create an instance of the SSQ module.
    /// </summary>
    public SsqCliModule(ITaskFactory taskFactory)
    {
        _taskFactory = taskFactory;
    }

    /// <inheritdoc />
    public string Name => "ssq";

    /// <inheritdoc />
    public string Description => "Handles SSQ format operations.";

    /// <inheritdoc />
    public IEnumerable<ICommand> Commands => new ICommand[]
    {
        new Command
        {
            Name = "decode",
            Description = "Decodes an SSQ file.",
            TaskFactory = Decode,
            Parameters = new[]
            {
                new CommandParameter
                {
                    Name = "-offset",
                    Description = "Global offset to add to output #OFFSET tag."
                }
            }
        }
    };

    /// <summary>
    /// Perform the DECODE command.
    /// </summary>
    private ITask Decode(Args args)
    {
        return _taskFactory
            .BuildDdrTask()
            .WithArgs(args)
            .CreateDecodeSsq();
    }
}