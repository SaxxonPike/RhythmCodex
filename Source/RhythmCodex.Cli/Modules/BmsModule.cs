using System.Collections.Generic;
using ClientCommon;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Modules;

[Service]
public class BmsModule(ITaskFactory taskFactory) : ICliModule
{
    public string Name => "bms";
    public string Description => "Manipulates BMS data.";

    public IEnumerable<ICommand> Commands => new ICommand[]
    {
        new Command()
        {
            Name = "render",
            Description = "Render a BMS to WAV.",
            TaskFactory = BmsRenderTask
        }
    };

    private ITask BmsRenderTask(Args args)
    {
        return taskFactory
            .BuildBmsTask()
            .WithArgs(args)
            .CreateRenderBms();
    }
}