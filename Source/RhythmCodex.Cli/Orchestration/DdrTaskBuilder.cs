using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Ddr.Converters;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Ddr.Streamers;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli.Orchestration
{
    public class DdrTaskBuilder : TaskBuilderBase<DdrTaskBuilder>
    {
        private readonly IDdr573StreamReader _ddr573StreamReader;
        private readonly IDdr573Decoder _ddr573Decoder;

        public DdrTaskBuilder(
            IFileSystem fileSystem,
            ILogger logger,
            IDdr573StreamReader ddr573StreamReader,
            IDdr573Decoder ddr573Decoder)
            : base(fileSystem, logger)
        {
            _ddr573StreamReader = ddr573StreamReader;
            _ddr573Decoder = ddr573Decoder;
        }

        public ITask CreateExtract()
        {
            return Build("Extract DDR 573 image",
                task =>
                {
                    task.Message = "Resolving input files.";

                    var inputFiles = GetInputFiles();
                    if (!inputFiles.Any())
                    {
                        Logger.Error("No input files.");
                        return false;
                    }

                    if (inputFiles.Length > 2)
                    {
                        Logger.Error("Using more than 2 input files is not supported yet.");
                        return false;
                    }

                    var gameImage = inputFiles[0];
                    var fileStreams = new List<FileStream>();
                    Ddr573Image image;

                    try
                    {
                        fileStreams.AddRange(inputFiles.Select(f => new FileStream(f, FileMode.Open, FileAccess.Read)));

                        image = fileStreams.Count == 1
                            ? _ddr573StreamReader.Read(fileStreams[0], (int) fileStreams[0].Length)
                            : _ddr573StreamReader.Read(fileStreams[0], (int) fileStreams[0].Length, fileStreams[1],
                                (int) fileStreams[1].Length);
                    }
                    finally
                    {
                        foreach (var fileStream in fileStreams)
                            fileStream?.Dispose();
                    }

                    task.Message = $"Decoding image.";
                    var files = _ddr573Decoder.Decode(image);

                    var fileIndex = 0;
                    foreach (var file in files)
                    {
                        task.Progress = fileIndex / (float) files.Count;
                        var outFileName = $"{file.Module:X4}{file.Offset:X7}.bin";
                        task.Message = $"Writing {outFileName}";
                        using (var stream = OpenWriteMulti(gameImage, _ => $"{file.Module:X4}{file.Offset:X7}.bin"))
                        {
                            var writer = new BinaryWriter(stream);
                            writer.Write(file.Data);
                            stream.Flush();
                        }
                    }

                    task.Message = "Finished.";
                    return true;
                });
        }
    }
}