using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Step2.Converters;
using RhythmCodex.Step2.Streamers;
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
    public class Step2CliModule : ICliModule
    {
        private readonly IFileSystem _fileSystem;
        private readonly IStep2StreamReader _step2StreamReader;
        private readonly ILogger _logger;
        private readonly ISmEncoder _smEncoder;
        private readonly ISmStreamWriter _smStreamWriter;
        private readonly IStep2Decoder _step2Decoder;

        /// <summary>
        /// Create an instance of the SSQ module.
        /// </summary>
        public Step2CliModule(
            ILogger logger,
            IStep2Decoder step2Decoder,
            ISmEncoder smEncoder,
            ISmStreamWriter smStreamWriter,
            IFileSystem fileSystem,
            IStep2StreamReader step2StreamReader)
        {
            _logger = logger;
            _step2Decoder = step2Decoder;
            _smEncoder = smEncoder;
            _smStreamWriter = smStreamWriter;
            _fileSystem = fileSystem;
            _step2StreamReader = step2StreamReader;
        }

        /// <inheritdoc />
        public string Name => "Step2";

        /// <inheritdoc />
        public string Description => "Decodes the Step2 format.";

        /// <inheritdoc />
        public IEnumerable<ICommand> Commands => new ICommand[]
        {
            new Command
            {
                Name = "decode",
                Description = "Decodes a Step2 file.",
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

                    var chunk = _step2StreamReader.Read(inFile, (int) inFile.Length);
                    var chart = _step2Decoder.Decode(chunk);
                    var encoded = _smEncoder.Encode(new ChartSet {Metadata = new Metadata(), Charts = new[] {chart}});

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