using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Extensions;
using RhythmCodex.ImaAdpcm.Converters;
using RhythmCodex.ImaAdpcm.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;

namespace RhythmCodex.Cli.Modules
{
    [Service]
    public class XboxModule : ICliModule
    {
        private readonly ITaskFactory _taskFactory;

        /// <summary>
        /// Create an instance of the Xbox module.
        /// </summary>
        public XboxModule(
            ITaskFactory taskFactory)
        {
            _taskFactory = taskFactory;
        }

        /// <inheritdoc />
        public string Name => "xbox";

        /// <inheritdoc />
        public string Description => "Handles conversion of Xbox native media.";

        /// <inheritdoc />
        public IEnumerable<ICommand> Commands => new ICommand[]
        {
            new Command
            {
                Name = "decode-adpcm",
                Description = "Decodes a raw blob of Xbox ADCPM data.",
                Execute = DecodeAdpcm
            },
            new Command
            {
                Name = "decode-dds",
                Description = "Decodes a DDS image.",
                Execute = DecodeDds
            }
        };

        private void DecodeDds(Args args)
        {
            _taskFactory
                .BuildXboxTask()
                .WithArgs(args)
                .CreateDecodeDds()
                .Run();
        }

        private void DecodeAdpcm(Args args)
        {
            _taskFactory
                .BuildXboxTask()
                .WithArgs(args)
                .CreateDecodeAdpcm()
                .Run();
        }
    }
}