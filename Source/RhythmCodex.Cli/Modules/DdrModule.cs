using System.Collections.Generic;
using ClientCommon;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.IoC;

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
                TaskFactory = Extract573Flash,
                Parameters = new []
                {
                    new CommandParameter
                    {
                        Description = "Decryption key for MDB.",
                        Name = "k"
                    }
                }
            },
            new Command
            {
                Name = "apply-sif",
                Description = "Applies SIF metadata to a SM file.",
                TaskFactory = ApplySif
            },
            new Command
            {
                Name = "decrypt-573-audio",
                Description = "Decrypts Digital 573 audio.",
                TaskFactory = Decrypt573Audio
            }
        };

        private ITask ApplySif(Args args)
        {
            return _taskFactory
                .BuildDdrTask()
                .WithArgs(args)
                .CreateApplySif();
        }

        private ITask Extract573Flash(Args args)
        {
            return _taskFactory
                .BuildDdrTask()
                .WithArgs(args)
                .CreateExtract();
        }

        private ITask Decrypt573Audio(Args args)
        {
            return _taskFactory
                .BuildDdrTask()
                .WithArgs(args)
                .CreateDecrypt573Audio();
        }
    }
}