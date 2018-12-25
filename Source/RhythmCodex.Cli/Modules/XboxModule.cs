using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Cli.Helpers;
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
        private readonly IArgResolver _argResolver;
        private readonly ILogger _logger;

        /// <summary>
        /// Create an instance of the Xbox module.
        /// </summary>
        public XboxModule(
            ILogger logger,
            IFileSystem fileSystem,
            IImaAdpcmDecoder imaAdpcmDecoder,
            IRiffPcm16SoundEncoder riffPcm16SoundEncoder,
            IRiffStreamWriter riffStreamWriter,
            IArgResolver argResolver)
        {
            _logger = logger;
            _fileSystem = fileSystem;
            _imaAdpcmDecoder = imaAdpcmDecoder;
            _riffPcm16SoundEncoder = riffPcm16SoundEncoder;
            _riffStreamWriter = riffStreamWriter;
            _argResolver = argResolver;
        }

        /// <inheritdoc />
        public string Name => "Xbox";

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