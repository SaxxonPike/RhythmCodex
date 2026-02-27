using System.Collections.Generic;
using ClientCommon;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Modules;

/// <summary>
/// A module which operates with the SSQ and other associated file formats.
/// </summary>
[Service]
public class Step1CliModule : ICliModule
{
    private readonly ITaskFactory _taskFactory;

    /// <summary>
    /// Create an instance of the SSQ module.
    /// </summary>
    public Step1CliModule(ITaskFactory taskFactory)
    {
        _taskFactory = taskFactory;
    }

    /// <inheritdoc />
    public string Name => "step";

    /// <inheritdoc />
    public string Description => "Encodes and decodes the STEP format. (2nd-3rdPlus, SoloBass)";

    /// <inheritdoc />
    public IEnumerable<ICommand> Commands =>
    [
        new Command
        {
            Name = "decode",
            Description = "Decodes a STEP file.",
            TaskFactory = Decode,
            Parameters =
            [
                new CommandParameter
                {
                    Name = "-offset",
                    Description = "Global offset to add to output #OFFSET tag."
                }
            ]
        }
    ];

    /// <summary>
    /// Perform the DECODE command.
    /// </summary>
    private ITask Decode(Args args)
    {
        return _taskFactory
            .BuildDdrTask()
            .WithArgs(args)
            .CreateDecodeStep1();
    }
}