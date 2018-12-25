using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
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
        private readonly IFileSystem _fileSystem;
        private readonly IArgResolver _argResolver;
        private readonly ILogger _logger;
        private readonly ISmEncoder _smEncoder;
        private readonly ISmStreamWriter _smStreamWriter;
        private readonly ISsqDecoder _ssqDecoder;
        private readonly ISsqStreamReader _ssqStreamReader;

        /// <summary>
        /// Create an instance of the SSQ module.
        /// </summary>
        public SsqCliModule(
            ILogger logger,
            ISsqDecoder ssqDecoder,
            ISsqStreamReader ssqStreamReader,
            ISmEncoder smEncoder,
            ISmStreamWriter smStreamWriter,
            IFileSystem fileSystem,
            IArgResolver argResolver)
        {
            _logger = logger;
            _ssqDecoder = ssqDecoder;
            _ssqStreamReader = ssqStreamReader;
            _smEncoder = smEncoder;
            _smStreamWriter = smStreamWriter;
            _fileSystem = fileSystem;
            _argResolver = argResolver;
        }

        /// <inheritdoc />
        public string Name => "Ssq";

        /// <inheritdoc />
        public string Description => "Encodes and decodes the SSQ format.";

        /// <inheritdoc />
        public IEnumerable<ICommand> Commands => new ICommand[]
        {
            new Command
            {
                Name = "encode",
                Description = "Encodes an SSQ file.",
                Execute = Encode,
                Parameters = new[]
                {
                    new CommandParameter { Name = "-o <path>", Description = "Sets an output path." }
                }
            },
            new Command
            {
                Name = "decode",
                Description = "Decodes an SSQ file.",
                Execute = Decode,
                Parameters = new[]
                {
                    new CommandParameter { Name = "-o <path>", Description = "Sets an output path." }
                }
            }
        };

        /// <summary>
        /// Perform the ENCODE command.
        /// </summary>
        private void Encode(Args args)
        {
            _logger.Warning("Todo: write encoder.");
        }

        /// <summary>
        /// Perform the DECODE command.
        /// </summary>
        private void Decode(Args args)
        {
            var outputDirectory = _argResolver.GetOutputDirectory(args);
            var inputFiles = _argResolver.GetInputFiles(args);
            
            foreach (var inputFile in inputFiles)
                _logger.Debug($"Input file: {inputFile}");            

            if (!inputFiles.Any())
            {
                _logger.Error("No input files.");
                return;
            }

            _logger.Info($"Using output directory: {outputDirectory}");
            _fileSystem.CreateDirectory(outputDirectory);

            foreach (var inputFile in inputFiles)
            {
                _logger.Info($"Converting {inputFile}");
                using (var inFile = _fileSystem.OpenRead(inputFile))
                {
                    var outFileName = _fileSystem.GetFileName(inputFile) + ".sm";
                    var outFilePath = _fileSystem.CombinePath(outputDirectory, outFileName);

                    var chunks = _ssqStreamReader.Read(inFile);
                    var charts = _ssqDecoder.Decode(chunks);
                    var encoded = _smEncoder.Encode(new ChartSet {Metadata = new Metadata(), Charts = charts});

                    _logger.Info($"Writing {outFileName}");
                    using (var outFile = _fileSystem.OpenWrite(outFilePath))
                    {
                        _smStreamWriter.Write(outFile, encoded);
                        outFile.Flush();
                    }
                }
            }
        }
    }
}