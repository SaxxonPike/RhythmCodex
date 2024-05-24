using System.Collections.Generic;
using ClientCommon;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Modules;

[Service]
public class ArcModule(ITaskFactory taskFactory) : ICliModule
{
    public string Name => "arc";
    public string Description => "Manipulates ARC archive files.";

    public IEnumerable<ICommand> Commands => new Command[]
    {
        new()
        {
            Name = "extract",
            Description = "Extracts files from an ARC archive.",
            TaskFactory = Extract
        }
    };

    private ITask Extract(Args args)
    {
        return taskFactory
            .BuildArcTask()
            .WithArgs(args)
            .CreateExtract();
    }
}