using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
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
    public class SsqCliModule : ICliModule
    {
        private readonly IFileSystem _fileSystem;
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
            IFileSystem fileSystem)
        {
            _logger = logger;
            _ssqDecoder = ssqDecoder;
            _ssqStreamReader = ssqStreamReader;
            _smEncoder = smEncoder;
            _smStreamWriter = smStreamWriter;
            _fileSystem = fileSystem;
        }

        /// <inheritdoc />
        public string Name => "SSQ";

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
        /// Get all input files from command line args.
        /// </summary>
        private string[] GetInputFiles(IDictionary<string, string[]> args)
        {
            var files = (args.ContainsKey(string.Empty)
                ? args[string.Empty].SelectMany(a => _fileSystem.GetFileNames(a)).ToArray()
                : Enumerable.Empty<string>()).ToArray();
            foreach (var inputFile in files)
                _logger.Debug($"Input file: {inputFile}");
            return files;
        }

        /// <summary>
        /// Get output directory from command line args.
        /// </summary>
        private string GetOutputDirectory(IDictionary<string, string[]> args)
        {
            var value = args.ContainsKey("o")
                ? args["o"].FirstOrDefault()
                : null;

            return !string.IsNullOrEmpty(value)
                ? value
                : _fileSystem.CurrentPath;
        }

        /// <summary>
        /// Perform the ENCODE command.
        /// </summary>
        private void Encode(IDictionary<string, string[]> args)
        {
            _logger.Warning("Todo: write encoder.");
        }

        /// <summary>
        /// Perform the DECODE command.
        /// </summary>
        private void Decode(IDictionary<string, string[]> args)
        {
            var outputDirectory = GetOutputDirectory(args);
            var inputFiles = GetInputFiles(args);

            if (!inputFiles.Any())
            {
                _logger.Error("No input files.");
                return;
            }

            _logger.Info($"Using output directory: {outputDirectory}");

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