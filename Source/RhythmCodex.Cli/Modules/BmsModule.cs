using System.Collections.Generic;
using ClientCommon;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Modules
{
    [Service]
    public class BmsModule : ICliModule
    {
        private readonly ITaskFactory _taskFactory;

        public BmsModule(ITaskFactory taskFactory)
        {
            _taskFactory = taskFactory;
        }

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
            return _taskFactory
                .BuildBmsTask()
                .WithArgs(args)
                .CreateRenderBms();
        }
    }
}