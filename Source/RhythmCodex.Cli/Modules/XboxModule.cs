using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly IFileSystem _fileSystem;
        private readonly IImaAdpcmDecoder _imaAdpcmDecoder;
        private readonly IRiffPcm16SoundEncoder _riffPcm16SoundEncoder;
        private readonly IRiffStreamWriter _riffStreamWriter;
        private readonly ILogger _logger;

        /// <summary>
        /// Create an instance of the Xbox module.
        /// </summary>
        public XboxModule(
            ILogger logger,
            IFileSystem fileSystem,
            IImaAdpcmDecoder imaAdpcmDecoder,
            IRiffPcm16SoundEncoder riffPcm16SoundEncoder,
            IRiffStreamWriter riffStreamWriter)
        {
            _logger = logger;
            _fileSystem = fileSystem;
            _imaAdpcmDecoder = imaAdpcmDecoder;
            _riffPcm16SoundEncoder = riffPcm16SoundEncoder;
            _riffStreamWriter = riffStreamWriter;
        }

        /// <inheritdoc />
        public string Name => "XBOX";

        /// <inheritdoc />
        public string Description => "Handles conversion of Xbox native media.";

        /// <inheritdoc />
        public IEnumerable<ICommand> Commands => new ICommand[]
        {
            new Command
            {
                Name = "decode",
                Description = "Decodes a raw blob of Xbox ADCPM data.",
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
            _fileSystem.CreateDirectory(outputDirectory);

            foreach (var inputFile in inputFiles)
            {
                var outputName = $"{Path.GetFileNameWithoutExtension(inputFile)}.wav";
                var outputPath = Path.Combine(outputDirectory, outputName);
                _logger.Info($"Converting {inputFile}");
                
                var sound = _imaAdpcmDecoder.Decode(new ImaAdpcmChunk
                {
                    Channels = 2,
                    ChannelSamplesPerFrame = 64,
                    Data = File.ReadAllBytes(inputFile),
                    Rate = 44100
                }).Single();
                
                _logger.Info($"Writing {outputPath}");

                var encoded = _riffPcm16SoundEncoder.Encode(sound);
                using (var outStream = new MemoryStream())
                {
                    _riffStreamWriter.Write(outStream, encoded);
                    outStream.Flush();
                    File.WriteAllBytes(outputPath, outStream.ToArray());
                }                    
            }
        }
    }
}