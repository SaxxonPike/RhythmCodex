using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Cli.Helpers;
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
        private readonly IArgResolver _argResolver;
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
            IStep2StreamReader step2StreamReader,
            IArgResolver argResolver)
        {
            _logger = logger;
            _step2Decoder = step2Decoder;
            _smEncoder = smEncoder;
            _smStreamWriter = smStreamWriter;
            _fileSystem = fileSystem;
            _step2StreamReader = step2StreamReader;
            _argResolver = argResolver;
        }

        /// <inheritdoc />
        public string Name => "Step2";

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