using System.Collections.Generic;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli.Modules
{
    [Service]
    public class BeatmaniaModule : ICliModule
    {
        private readonly ITaskFactory _taskFactory;

        /// <summary>
        /// Create an instance of the 573 module.
        /// </summary>
        public BeatmaniaModule(
            ITaskFactory taskFactory)
        {
            _taskFactory = taskFactory;
        }

        /// <inheritdoc />
        public string Name => "bm";

        /// <inheritdoc />
        public string Description => "Manipulates Beatmania AC data.";

        /// <inheritdoc />
        public IEnumerable<ICommand> Commands => new ICommand[]
        {
            new Command
            {
                Name = "extract-2dx",
                Description = "Extracts sound files from a 2DX file.",
                Execute = Extract2dx
            },
            new Command
            {
                Name = "decode-1",
                Description = "Decode a .1 file chart set.",
                Execute = Decode1,
                Parameters = new[]
                {
                    new CommandParameter
                    {
                        Name = "rate",
                        Description = "Ticks per second. Pre-GOLD uses 59.94. GOLD uses 60.94. Default is 1000."
                    }
                }
            },
        };

        private void Decode1(Args args)
        {
            _taskFactory
                .BuildBeatmaniaTask()
                .WithArgs(args)
                .CreateDecode1()
                .Run();
        }

        private void Extract2dx(Args args)
        {
            _taskFactory
                .BuildBeatmaniaTask()
                .WithArgs(args)
                .CreateExtract2dx()
                .Run();
        }
    }
}