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
                Name = "decode-xst",
                Description = "Decodes a raw blob of Xbox ADCPM data.",
                Execute = DecodeAdpcm
            },
            new Command
            {
                Name = "decode-dds",
                Description = "Decodes a DDS image.",
                Execute = DecodeDds
            },
            new Command
            {
                Name = "extract-xwb",
                Description = "Extracts an XWB sound bank.",
                Execute = ExtractXwb
            },
            new Command
            {
                Name = "extract-iso",
                Description = "Extracts files from an Xbox ISO.",
                Execute = ExtractXiso
            },
            new Command
            {
                Name = "extract-sng",
                Description = "Extracts songs from an SNG file.",
                Execute = ExtractSng
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
                .CreateDecodeXst()
                .Run();
        }

        private void ExtractXwb(Args args)
        {
            _taskFactory
                .BuildXboxTask()
                .WithArgs(args)
                .CreateExtractXwb()
                .Run();
        }

        private void ExtractXiso(Args args)
        {
            _taskFactory
                .BuildXboxTask()
                .WithArgs(args)
                .CreateExtractXiso()
                .Run();
        }

        private void ExtractSng(Args args)
        {
            _taskFactory
                .BuildXboxTask()
                .WithArgs(args)
                .CreateExtractSng()
                .Run();
        }
    }
}