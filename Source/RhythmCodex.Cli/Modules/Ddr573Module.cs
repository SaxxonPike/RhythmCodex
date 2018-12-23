using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Ddr.Converters;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Ddr.Streamers;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli.Modules
{
    [Service]
    public class Ddr573Module : ICliModule
    {
        private readonly IFileSystem _fileSystem;
        private readonly IDdr573Decoder _ddr573Decoder;
        private readonly IDdr573StreamReader _ddr573StreamReader;
        private readonly ILogger _logger;

        /// <summary>
        /// Create an instance of the 573 module.
        /// </summary>
        public Ddr573Module(
            ILogger logger,
            IFileSystem fileSystem,
            IDdr573Decoder ddr573Decoder,
            IDdr573StreamReader ddr573StreamReader)
        {
            _logger = logger;
            _fileSystem = fileSystem;
            _ddr573Decoder = ddr573Decoder;
            _ddr573StreamReader = ddr573StreamReader;
        }

        /// <inheritdoc />
        public string Name => "DDR573";

        /// <inheritdoc />
        public string Description => "Manipulates flash images for 573-based DDR. (1stMix-8thMix)";

        /// <inheritdoc />
        public IEnumerable<ICommand> Commands => new ICommand[]
        {
            new Command
            {
                Name = "extract",
                Description = "Extracts files from a GAME (and optionally additionally CARD) image.",
                Execute = Extract,
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
        /// Perform the EXTRACT command.
        /// </summary>
        private void Extract(IDictionary<string, string[]> args)
        {
            var outputDirectory = GetOutputDirectory(args);
            var inputFiles = GetInputFiles(args);

            if (!inputFiles.Any())
            {
                _logger.Error("No input files.");
                return;
            }

            if (inputFiles.Length > 2)
            {
                _logger.Error("Using more than 2 input files is not supported yet.");
                return;
            }
            
            _logger.Info($"Using GAME image: {inputFiles[0]}");

            if (inputFiles.Length > 1)
                _logger.Info($"Using CARD image: {inputFiles[1]}");

            _logger.Info($"Using output directory: {outputDirectory}");
            _fileSystem.CreateDirectory(outputDirectory);

            var fileStreams = new List<FileStream>();
            Ddr573Image image;

            try
            {
                fileStreams.AddRange(inputFiles.Select(f => new FileStream(f, FileMode.Open, FileAccess.Read)));
                
                image = fileStreams.Count == 1 
                    ? _ddr573StreamReader.Read(fileStreams[0], (int)fileStreams[0].Length) 
                    : _ddr573StreamReader.Read(fileStreams[0], (int)fileStreams[0].Length, fileStreams[1], (int)fileStreams[1].Length);
            }
            finally
            {
                foreach (var fileStream in fileStreams)
                    fileStream?.Dispose();
            }

            var files = _ddr573Decoder.Decode(image);

            foreach (var file in files)
            {
                var outFileName = $"{file.Module:X4}{file.Offset:X7}.bin";
                var outFilePath = _fileSystem.CombinePath(outputDirectory, outFileName);

                _logger.Info($"Writing {outFileName}");
                _fileSystem.WriteAllBytes(outFilePath, file.Data);
            }
        }
    }
}