using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Cli.Helpers;
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
        private readonly IArgResolver _argResolver;
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
            IFileSystem fileSystem,
            IArgResolver argResolver)
        {
            _logger = logger;
            _step1Decoder = step1Decoder;
            _step1StreamReader = step1StreamReader;
            _smEncoder = smEncoder;
            _smStreamWriter = smStreamWriter;
            _fileSystem = fileSystem;
            _argResolver = argResolver;
        }

        /// <inheritdoc />
        public string Name => "Step";

        /// <inheritdoc />
        public string Description => "Encodes and decodes the STEP format. (2nd-3rdPlus, SoloBass)";

        /// <inheritdoc />
        public IEnumerable<ICommand> Commands => new ICommand[]
        {
            new Command
            {
                Name = "decode",
                Description = "Decodes a STEP file.",
                Execute = Decode
            }
        };

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