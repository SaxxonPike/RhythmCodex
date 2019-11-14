using System.Collections.Generic;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Modules
{
    // ReSharper disable once ClassNeverInstantiated.Global
    /// <summary>
    /// A module which operates with the SSQ and other associated file formats.
    /// </summary>
    [Service]
    public class Step2CliModule : ICliModule
    {
        private readonly ITaskFactory _taskFactory;

        /// <summary>
        /// Create an instance of the SSQ module.
        /// </summary>
        public Step2CliModule(
            ITaskFactory taskFactory)
        {
            _taskFactory = taskFactory;
        }

        /// <inheritdoc />
        public string Name => "step2";

        /// <inheritdoc />
        public string Description => "Decodes the STEP2 format. (1stMix)";

        /// <inheritdoc />
        public IEnumerable<ICommand> Commands => new ICommand[]
        {
            new Command
            {
                Name = "decode",
                Description = "Decodes a STEP2 file.",
                Execute = Decode
            }
        };

        /// <summary>
        /// Perform the DECODE command.
        /// </summary>
        private void Decode(Args args)
        {
            _taskFactory
                .BuildDdrTask()
                .WithArgs(args)
                .CreateDecodeStep2()
                .Run();
        }
    }
}