using System;
using System.Collections.Generic;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Modules
{
    [Service]
    public class ArcModule : ICliModule
    {
        private readonly ITaskFactory _taskFactory;

        public ArcModule(
            ITaskFactory taskFactory)
        {
            _taskFactory = taskFactory;
        }
        
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
            return _taskFactory
                .BuildArcTask()
                .WithArgs(args)
                .CreateExtract();
        }
    }
}