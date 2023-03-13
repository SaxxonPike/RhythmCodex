using System.Collections.Generic;
using ClientCommon;
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
                TaskFactory = CompressBemaniLz
            },
            new Command
            {
                Name = "decompress",
                Description = "Decompress a file with Bemani LZ.",
                TaskFactory = DecompressBemaniLz
            }
        };

        private ITask CompressBemaniLz(Args args)
        {
            return _taskFactory
                .BuildCompressionTask()
                .WithArgs(args)
                .CreateCompressBemaniLz();
        }

        private ITask DecompressBemaniLz(Args args)
        {
            return _taskFactory
                .BuildCompressionTask()
                .WithArgs(args)
                .CreateDecompressBemaniLz();
        }
    }
}