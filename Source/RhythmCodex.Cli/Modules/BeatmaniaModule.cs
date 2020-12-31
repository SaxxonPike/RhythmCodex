using System.Collections.Generic;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.IoC;

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
                Name = "decode-djmain-hdd",
                Description = "Extracts and decodes BMS files from a DJMAIN hard drive image.",
                TaskFactory = DecodeDjmainHdd,
                Parameters = new[]
                {
                    new CommandParameter
                    {
                        Name = "+noaudio",
                        Description = "Disable extracting audio."
                    },
                    new CommandParameter
                    {
                        Name = "+nochart",
                        Description = "Disable extracting converted charts."
                    },
                    new CommandParameter
                    {
                        Name = "+raw",
                        Description = "Enable saving the original encoded chart."
                    }
                }
            },
            new Command
            {
                Name = "render-djmain-gst",
                Description = "Renders all charts on a DJMAIN hard drive image to WAV.",
                TaskFactory = RenderDjmainGst
            },
            new Command
            {
                Name = "extract-2dx",
                Description = "Extracts sound files from a 2DX file.",
                TaskFactory = Extract2dx
            },
            new Command
            {
                Name = "decode-1",
                Description = "Decode a .1 file chart set.",
                TaskFactory = Decode1,
                Parameters = new[]
                {
                    new CommandParameter
                    {
                        Name = "-rate",
                        Description = "Ticks per second. Pre-GOLD uses 59.94. GOLD uses 60.94. Default is 1000."
                    }
                }
            },
        };

        private ITask RenderDjmainGst(Args args)
        {
            return _taskFactory
                .BuildBeatmaniaTask()
                .WithArgs(args)
                .CreateRenderDjmainGst();
        }

        private ITask DecodeDjmainHdd(Args args)
        {
            return _taskFactory
                .BuildBeatmaniaTask()
                .WithArgs(args)
                .CreateDecodeDjmainHdd();
        }

        private ITask Decode1(Args args)
        {
            return _taskFactory
                .BuildBeatmaniaTask()
                .WithArgs(args)
                .CreateDecode1();
        }

        private ITask Extract2dx(Args args)
        {
            return _taskFactory
                .BuildBeatmaniaTask()
                .WithArgs(args)
                .CreateExtract2dx();
        }
    }
}