using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Ssq.Streamers;
using RhythmCodex.Step1.Converters;
using RhythmCodex.Step1.Streamers;
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
    public class Step1CliModule : ICliModule
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly ISmEncoder _smEncoder;
        private readonly ISmStreamWriter _smStreamWriter;
        private readonly IStep1Decoder _step1Decoder;
        private readonly IStep1StreamReader _step1StreamReader;

        /// <summary>
        /// Create an instance of the SSQ module.
        /// </summary>
        public Step1CliModule(
            ILogger logger,
            IStep1Decoder step1Decoder,
            IStep1StreamReader step1StreamReader,
            ISmEncoder smEncoder,
            ISmStreamWriter smStreamWriter,
            IFileSystem fileSystem)
        {
            _logger = logger;
            _step1Decoder = step1Decoder;
            _step1StreamReader = step1StreamReader;
            _smEncoder = smEncoder;
            _smStreamWriter = smStreamWriter;
            _fileSystem = fileSystem;
        }

        /// <inheritdoc />
        public string Name => "STEP";

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
                Parameters = new[]
                {
                    new CommandParameter {Name = "-o <path>", Description = "Sets an output path."}
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

                    var chunks = _step1StreamReader.Read(inFile);
                    var charts = _step1Decoder.Decode(chunks);
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