using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Ddr.Converters;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Ddr.Streamers;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli.Modules
{
    [Service]
    public class DdrModule : ICliModule
    {
        private readonly ITaskFactory _taskFactory;

        /// <summary>
        /// Create an instance of the 573 module.
        /// </summary>
        public DdrModule(
            ITaskFactory taskFactory)
        {
            _taskFactory = taskFactory;
        }

        /// <inheritdoc />
        public string Name => "ddr";

        /// <inheritdoc />
        public string Description => "Manipulates DDR AC data.";

        /// <inheritdoc />
        public IEnumerable<ICommand> Commands => new ICommand[]
        {
            new Command
            {
                Name = "extract-573-flash",
                Description = "Extracts files from a 573 GAME (and optionally additionally CARD) image.",
                Execute = Extract573Flash
            },
            new Command
            {
                Name = "apply-sif",
                Description = "Applies SIF metadata to a SM file.",
                Execute = ApplySif
            }
        };

        private void ApplySif(Args args)
        {
            _taskFactory
                .BuildDdrTask()
                .WithArgs(args)
                .CreateApplySif()
                .Run();
        }

        private void Extract573Flash(Args args)
        {
            _taskFactory
                .BuildDdrTask()
                .WithArgs(args)
                .CreateExtract()
                .Run();
        }
    }
}