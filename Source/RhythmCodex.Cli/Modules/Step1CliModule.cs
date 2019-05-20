using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Ssq.Streamers;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Cli.Modules
{
    // ReSharper disable once ClassNeverInstantiated.Global
    /// <summary>
    /// A module which operates with the SSQ and other associated file formats.
    /// </summary>
    [Service]
    public class Step1CliModule : ICliModule
    {
        private readonly ITaskFactory _taskFactory;

        /// <summary>
        /// Create an instance of the SSQ module.
        /// </summary>
        public Step1CliModule(ITaskFactory taskFactory)
        {
            _taskFactory = taskFactory;
        }

        /// <inheritdoc />
        public string Name => "step";

        /// <inheritdoc />
        public string Description => "Encodes and decodes the STEP format. (2nd-3rdPlus, SoloBass)";

        /// <inheritdoc />
        public IEnumerable<ICommand> Commands => new ICommand[]
        {
            new Command
            {
                Name = "decode",
                Description = "Decodes a STEP file.",
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
                .CreateDecodeStep1()
                .Run();
        }
    }
}