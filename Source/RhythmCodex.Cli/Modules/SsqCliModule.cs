using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Ssq.Streamers;
using RhythmCodex.Stepmania.Converters;
using RhythmCodex.Stepmania.Model;
using RhythmCodex.Stepmania.Streamers;
using RhythmCodex.Extensions;

namespace RhythmCodex.Cli.Modules
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SsqCliModule : ICliModule
    {
        private readonly ILogger _logger;
        private readonly ISsqDecoder _ssqDecoder;
        private readonly ISsqStreamReader _ssqStreamReader;
        private readonly ISmEncoder _smEncoder;
        private readonly ISmStreamWriter _smStreamWriter;
        private readonly IFileSystem _fileSystem;

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

        public string Name => "SSQ";

        public string Description => "Encodes and decodes the SSQ format.";

        public IEnumerable<ICommand> Commands => new ICommand[]
        {
            new Command
            {
                Name = "encode",
                Description = "Encodes an SSQ file.",
                Execute = Encode
            },
            new Command
            {
                Name = "decode",
                Description = "Decodes an SSQ file.",
                Execute = Decode
            }
        };

        private string[] GetInputFiles(IDictionary<string, string[]> args)
        {
            var files = (args.ContainsKey(string.Empty)
                ? args[string.Empty].SelectMany(a => _fileSystem.GetFileNames(a)).ToArray()
                : Enumerable.Empty<string>()).ToArray();
            foreach (var inputFile in files)
            {
                _logger.Debug($"Input file: {inputFile}");
            }
            return files;
        }

        private string GetOutputDirectory(IDictionary<string, string[]> args)
        {
            var value = args.ContainsKey("o") 
                ? args["o"].FirstOrDefault() 
                : null;
            
            return !string.IsNullOrEmpty(value) 
                ? value 
                : _fileSystem.CurrentPath;
        }

        private void Encode(IDictionary<string, string[]> args)
        {
            _logger.Warning("Todo: write encoder.");
        }

        private void Decode(IDictionary<string, string[]> args)
        {
            var outputDirectory = GetOutputDirectory(args);
            var inputFiles = GetInputFiles(args);

            if (!inputFiles.Any())
            {
                _logger.Error("No input files specified.");
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
                    _logger.Debug($"Found {chunks.Count} total chunks");
                    var charts = _ssqDecoder.Decode(chunks);
                    _logger.Debug($"Found {charts.Count} charts");

                    _logger.Debug("Encoding SM");
                    var encoded = _smEncoder.Encode(new ChartSet { Metadata = new Metadata(), Charts = charts });

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