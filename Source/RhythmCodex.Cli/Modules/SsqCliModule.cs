using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Ssq.Streamers;
using RhythmCodex.Stepmania.Converters;
using RhythmCodex.Stepmania.Model;
using RhythmCodex.Stepmania.Streamers;

namespace RhythmCodex.Cli.Modules
{
    // ReSharper disable once ClassNeverInstantiated.Global
    /// <summary>
    /// A module which operates with the SSQ and other associated file formats.
    /// </summary>
    [Service]
    public class SsqCliModule : ICliModule
    {
        private readonly ITaskFactory _taskFactory;

        /// <summary>
        /// Create an instance of the SSQ module.
        /// </summary>
        public SsqCliModule(ITaskFactory taskFactory)
        {
            _taskFactory = taskFactory;
        }

        /// <inheritdoc />
        public string Name => "ssq";

        /// <inheritdoc />
        public string Description => "Handles SSQ format operations.";

        /// <inheritdoc />
        public IEnumerable<ICommand> Commands => new ICommand[]
        {
            new Command
            {
                Name = "decode",
                Description = "Decodes an SSQ file.",
                Execute = Decode,
                Parameters = new []
                {
                    new CommandParameter
                    {
                        Name = "-offset",
                        Description = "Global offset to add to output #OFFSET tag."
                    }
                }
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
                .CreateDecodeSsq()
                .Run();
        }
    }
}