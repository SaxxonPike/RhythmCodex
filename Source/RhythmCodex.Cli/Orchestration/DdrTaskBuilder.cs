using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Ddr.Converters;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Ddr.Streamers;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Ssq.Streamers;
using RhythmCodex.Step1.Converters;
using RhythmCodex.Step1.Streamers;
using RhythmCodex.Step2.Converters;
using RhythmCodex.Step2.Streamers;
using RhythmCodex.Stepmania;
using RhythmCodex.Stepmania.Converters;
using RhythmCodex.Stepmania.Model;
using RhythmCodex.Stepmania.Streamers;

namespace RhythmCodex.Cli.Orchestration
{
    [InstancePerDependency]
    public class DdrTaskBuilder : TaskBuilderBase<DdrTaskBuilder>
    {
        private readonly IDdr573StreamReader _ddr573StreamReader;
        private readonly IDdr573Decoder _ddr573Decoder;
        private readonly ISsqStreamReader _ssqStreamReader;
        private readonly ISsqDecoder _ssqDecoder;
        private readonly ISmEncoder _smEncoder;
        private readonly ISmStreamWriter _smStreamWriter;
        private readonly IStep1StreamReader _step1StreamReader;
        private readonly IStep1Decoder _step1Decoder;
        private readonly IStep2StreamReader _step2StreamReader;
        private readonly IStep2Decoder _step2Decoder;
        private readonly IMetadataAggregator _metadataAggregator;
        private readonly ISmDecoder _smDecoder;
        private readonly ISifStreamReader _sifStreamReader;
        private readonly ISmStreamReader _smStreamReader;
        private readonly ISmMetadataChanger _smMetadataChanger;

        public DdrTaskBuilder(
            IFileSystem fileSystem,
            ILogger logger,
            IDdr573StreamReader ddr573StreamReader,
            IDdr573Decoder ddr573Decoder,
            ISsqStreamReader ssqStreamReader,
            ISsqDecoder ssqDecoder,
            ISmEncoder smEncoder,
            ISmStreamWriter smStreamWriter,
            IStep1StreamReader step1StreamReader,
            IStep1Decoder step1Decoder,
            IStep2StreamReader step2StreamReader,
            IStep2Decoder step2Decoder,
            IMetadataAggregator metadataAggregator,
            ISmDecoder smDecoder,
            ISifStreamReader sifStreamReader,
            ISmStreamReader smStreamReader,
            ISmMetadataChanger smMetadataChanger)
            : base(fileSystem, logger)
        {
            _ddr573StreamReader = ddr573StreamReader;
            _ddr573Decoder = ddr573Decoder;
            _ssqStreamReader = ssqStreamReader;
            _ssqDecoder = ssqDecoder;
            _smEncoder = smEncoder;
            _smStreamWriter = smStreamWriter;
            _step1StreamReader = step1StreamReader;
            _step1Decoder = step1Decoder;
            _step2StreamReader = step2StreamReader;
            _step2Decoder = step2Decoder;
            _metadataAggregator = metadataAggregator;
            _smDecoder = smDecoder;
            _sifStreamReader = sifStreamReader;
            _smStreamReader = smStreamReader;
            _smMetadataChanger = smMetadataChanger;
        }

        public ITask CreateDecodeSsq()
        {
            return Build("Decode SSQ", task =>
            {
                var files = GetInputFiles(task);
                if (!files.Any())
                {
                    task.Message = "No input files.";
                    return false;
                }

                ParallelProgress(task, files, file =>
                {
                    using (var inFile = OpenRead(task, file))
                    {
                        var chunks = _ssqStreamReader.Read(inFile);
                        var charts = _ssqDecoder.Decode(chunks);
                        var aggregatedInfo = _metadataAggregator.Aggregate(charts);
                        var title = Path.GetFileNameWithoutExtension(file.Name);

                        // This is a temporary hack to make building sets easier for right now
                        // TODO: make this optional via command line switch
                        if (title.EndsWith("_all", StringComparison.InvariantCultureIgnoreCase))
                            title = title.Substring(0, title.Length - 4);

                        var encoded = _smEncoder.Encode(new ChartSet
                        {
                            Metadata = new Metadata
                            {
                                [StringData.Title] = aggregatedInfo[StringData.Title] ?? title,
                                [StringData.Subtitle] = aggregatedInfo[StringData.Subtitle],
                                [StringData.Artist] = aggregatedInfo[StringData.Artist],
                                [ChartTag.MusicTag] = aggregatedInfo[StringData.Music] ?? $"{title}.ogg"
                            },
                            Charts = charts
                        });

                        using (var outFile = OpenWriteSingle(task, file, i => $"{i}.sm"))
                        {
                            _smStreamWriter.Write(outFile, encoded);
                            outFile.Flush();
                        }
                    }
                });

                return true;
            });
        }

        public ITask CreateDecodeStep1()
        {
            return Build("Decode STEP", task =>
            {
                var inputFiles = GetInputFiles(task);
                if (!inputFiles.Any())
                {
                    task.Message = "No input files.";
                    return false;
                }

                foreach (var inputFile in inputFiles)
                {
                    using (var inFile = OpenRead(task, inputFile))
                    {
                        var chunks = _step1StreamReader.Read(inFile);
                        var charts = _step1Decoder.Decode(chunks);
                        var encoded = _smEncoder.Encode(new ChartSet {Metadata = new Metadata(), Charts = charts});

                        using (var outFile = OpenWriteSingle(task, inputFile, i => $"{i}.sm"))
                        {
                            _smStreamWriter.Write(outFile, encoded);
                            outFile.Flush();
                        }
                    }
                }

                return true;
            });
        }

        public ITask CreateDecodeStep2()
        {
            return Build("Decode STEP2", task =>
            {
                var inputFiles = GetInputFiles(task);
                if (!inputFiles.Any())
                {
                    task.Message = "No input files.";
                    return false;
                }

                foreach (var inputFile in inputFiles)
                {
                    using (var inFile = OpenRead(task, inputFile))
                    {
                        var chunks = _step2StreamReader.Read(inFile, (int) inFile.Length);
                        var chart = _step2Decoder.Decode(chunks);
                        var encoded = _smEncoder.Encode(new ChartSet
                            {Metadata = new Metadata(), Charts = new[] {chart}});

                        using (var outFile = OpenWriteSingle(task, inputFile, i => $"{i}.sm"))
                        {
                            _smStreamWriter.Write(outFile, encoded);
                            outFile.Flush();
                        }
                    }
                }

                return true;
            });
        }

        public ITask CreateApplySif()
        {
            return Build("Apply SIF metadata", task =>
            {
                var inputFiles = GetInputFiles(task);
                if (!inputFiles.Any())
                {
                    task.Message = "No input files.";
                    return false;
                }

                foreach (var inputFile in inputFiles)
                {
                    using (var inFile = OpenRead(task, inputFile))
                    using (var smFile = OpenRelatedRead(inputFile, i => $"{i}_all.sm"))
                    {
                        var sm = _smStreamReader.Read(smFile).ToList();
                        var sif = _sifStreamReader.Read(inFile, inFile.Length).KeyValues;
                        var name = Path.GetFileNameWithoutExtension(inputFile.Name);

                        if (sif.ContainsKey("dir"))
                            name = sif["dir"];
                        if (sif.ContainsKey("title"))
                            _smMetadataChanger.SetTitle(sm, sif["title"]);
                        if (sif.ContainsKey("mix"))
                            _smMetadataChanger.SetSubtitle(sm, sif["mix"]);
                        if (sif.ContainsKey("artist"))
                            _smMetadataChanger.SetArtist(sm, sif["artist"]);
                        if (sif.ContainsKey("extra"))
                            _smMetadataChanger.SetSubartist(sm, sif["extra"]);
                        if (sif.ContainsKey("bpm_min") && sif.ContainsKey("bpm_max"))
                            _smMetadataChanger.SetBpm(sm, sif["bpm_min"], sif["bpm_max"]);

                        if (sif.ContainsKey("foot.single"))
                        {
                            var values = sif["foot.single"].Split(',');
                            _smMetadataChanger.SetDifficulty(sm, "dance-single", "easy", values[0]);
                            _smMetadataChanger.SetDifficulty(sm, "dance-single", "medium", values[1]);
                            _smMetadataChanger.SetDifficulty(sm, "dance-single", "hard", values[2]);
                        }
                        
                        if (sif.ContainsKey("foot.double"))
                        {
                            var values = sif["foot.double"].Split(',');
                            _smMetadataChanger.SetDifficulty(sm, "dance-double", "easy", values[0]);
                            _smMetadataChanger.SetDifficulty(sm, "dance-double", "medium", values[1]);
                            _smMetadataChanger.SetDifficulty(sm, "dance-double", "hard", values[2]);
                        }
                        
                        smFile.Dispose();

                        using (var outStream = OpenWriteSingle(task, inputFile, i => $"{name}_all.sm"))
                        {
                            _smStreamWriter.Write(outStream, sm);
                            outStream.Flush();
                        }
                    }
                }

                return true;
            });
        }

        public ITask CreateExtract()
        {
            return Build("Extract DDR 573 flash image",
                task =>
                {
                    var inputFiles = GetInputFiles(task);
                    if (!inputFiles.Any())
                    {
                        task.Message = "No input files.";
                        return false;
                    }

                    if (inputFiles.Length > 2)
                    {
                        task.Message = "Using more than 2 input files is not supported yet.";
                        return false;
                    }

                    var gameImage = inputFiles[0];
                    var fileStreams = new List<Stream>();
                    Ddr573Image image;

                    try
                    {
                        fileStreams.AddRange(inputFiles.Select(f => f.Open()));

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

                    var files = _ddr573Decoder.Decode(image);
                    var fileIndex = 0;
                    ParallelProgress(task, files, file =>
                    {
                        task.Progress = fileIndex / (float) files.Count;
                        var outFileName = $"{file.Module:X4}{file.Offset:X7}.bin";
                        task.Message = $"Writing {outFileName}";
                        using (var stream =
                            OpenWriteMulti(task, gameImage, _ => $"{file.Module:X4}{file.Offset:X7}.bin"))
                        {
                            var writer = new BinaryWriter(stream);
                            writer.Write(file.Data);
                            stream.Flush();
                        }
                    });

                    task.Message = "Finished.";
                    return true;
                });
        }
    }
}