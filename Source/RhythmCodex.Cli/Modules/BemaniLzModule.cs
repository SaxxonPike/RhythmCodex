using System.Collections.Generic;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Modules
{
    [Service]
    public class BemaniLzModule : ICliModule
    {
        private readonly ITaskFactory _taskFactory;

        /// <summary>
        /// Create an instance of the 573 module.
        /// </summary>
        public BemaniLzModule(
            ITaskFactory taskFactory)
        {
            _taskFactory = taskFactory;
        }

        /// <inheritdoc />
        public string Name => "bemanilz";

        /// <inheritdoc />
        public string Description => "Compress and decompress data using the Bemani LZ algorithm.";

        /// <inheritdoc />
        public IEnumerable<ICommand> Commands => new ICommand[]
        {
            new Command
            {
                Name = "compress",
                Description = "Compress a file with Bemani LZ.",
                Execute = CompressBemaniLz
            },
            new Command
            {
                Name = "decompress",
                Description = "Decompress a file with Bemani LZ.",
                Execute = DecompressBemaniLz
            }
        };

        private void CompressBemaniLz(Args args)
        {
            _taskFactory
                .BuildCompressionTask()
                .WithArgs(args)
                .CreateCompressBemaniLz()
                .Run();
        }

        private void DecompressBemaniLz(Args args)
        {
            _taskFactory
                .BuildCompressionTask()
                .WithArgs(args)
                .CreateDecompressBemaniLz()
                .Run();
        }
    }
}