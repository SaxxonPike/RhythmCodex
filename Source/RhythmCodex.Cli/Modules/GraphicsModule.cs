using System.Collections.Generic;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Modules
{
    [Service]
    public class GraphicsModule : ICliModule
    {
        private readonly ITaskFactory _taskFactory;

        /// <summary>
        /// Create an instance of the graphics module.
        /// </summary>
        public GraphicsModule(
            ITaskFactory taskFactory)
        {
            _taskFactory = taskFactory;
        }

        /// <inheritdoc />
        public string Name => "gfx";

        /// <inheritdoc />
        public string Description => "Handles conversion of standard graphics formats.";

        /// <inheritdoc />
        public IEnumerable<ICommand> Commands => new ICommand[]
        {
            new Command
            {
                Name = "decode-dds",
                Description = "Decodes a DDS image.",
                Execute = DecodeDds
            },
            new Command
            {
                Name = "decode-tga",
                Description = "Decodes a TGA image.",
                Execute = DecodeTga
            },
            new Command
            {
                Name = "decode-tim",
                Description = "Decodes a TIM image.",
                Execute = DecodeTim
            }
        };

        private void DecodeTim(Args args)
        {
            _taskFactory
                .BuildGraphicsTask()
                .WithArgs(args)
                .CreateDecodeTim()
                .Run();
        }

        private void DecodeDds(Args args)
        {
            _taskFactory
                .BuildGraphicsTask()
                .WithArgs(args)
                .CreateDecodeDds()
                .Run();
        }

        private void DecodeTga(Args args)
        {
            _taskFactory
                .BuildGraphicsTask()
                .WithArgs(args)
                .CreateDecodeTga()
                .Run();
        }
    }
}