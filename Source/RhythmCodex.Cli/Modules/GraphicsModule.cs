using System.Collections.Generic;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Infrastructure;

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
                Name = "decode-tga",
                Description = "Decodes a TGA image.",
                Execute = DecodeTga
            }
        };

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