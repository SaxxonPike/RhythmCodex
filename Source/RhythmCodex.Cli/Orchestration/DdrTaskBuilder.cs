using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Ddr.Converters;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Ddr.Providers;
using RhythmCodex.Ddr.Streamers;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sif.Converters;
using RhythmCodex.Sif.Streamers;
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
    [Service(singleInstance: false)]
    public class DdrTaskBuilder : TaskBuilderBase<DdrTaskBuilder>
    {
        private readonly IDdr573ImageStreamReader _ddr573ImageStreamReader;
        private readonly IDdr573ImageDecoder _ddr573ImageDecoder;
        private readonly ISsqStreamReader _ssqStreamReader;
        private readonly ISsqDecoder _ssqDecoder;
        private readonly ISmEncoder _smEncoder;
        private readonly ISmStreamWriter _smStreamWriter;
        private readonly IStep1StreamReader _step1StreamReader;
        private readonly IStep1Decoder _step1Decoder;
        private readonly IStep2StreamReader _step2StreamReader;
        private readonly IStep2Decoder _step2Decoder;
        private readonly IMetadataAggregator _metadataAggregator;
        private readonly ISifStreamReader _sifStreamReader;
        private readonly ISmStreamReader _smStreamReader;
        private readonly ISifSmMetadataChanger _sifSmMetadataChanger;
        private readonly IHeuristicTester _heuristicTester;
        private readonly IDdr573AudioKeyProvider _ddr573AudioKeyProvider;
        private readonly IDdr573AudioDecrypter _ddr573AudioDecrypter;

        public DdrTaskBuilder(
            IFileSystem fileSystem,
            ILogger logger,
            IDdr573ImageStreamReader ddr573ImageStreamReader,
            IDdr573ImageDecoder ddr573ImageDecoder,
            ISsqStreamReader ssqStreamReader,
            ISsqDecoder ssqDecoder,
            ISmEncoder smEncoder,
            ISmStreamWriter smStreamWriter,
            IStep1StreamReader step1StreamReader,
            IStep1Decoder step1Decoder,
            IStep2StreamReader step2StreamReader,
            IStep2Decoder step2Decoder,
            IMetadataAggregator metadataAggregator,
            ISifStreamReader sifStreamReader,
            ISmStreamReader smStreamReader,
            ISifSmMetadataChanger sifSmMetadataChanger,
            IHeuristicTester heuristicTester,
            IDdr573AudioKeyProvider ddr573AudioKeyProvider,
            IDdr573AudioDecrypter ddr573AudioDecrypter)
            : base(fileSystem, logger)
        {
            _ddr573ImageStreamReader = ddr573ImageStreamReader;
            _ddr573ImageDecoder = ddr573ImageDecoder;
            _ssqStreamReader = ssqStreamReader;
            _ssqDecoder = ssqDecoder;
            _smEncoder = smEncoder;
            _smStreamWriter = smStreamWriter;
            _step1StreamReader = step1StreamReader;
            _step1Decoder = step1Decoder;
            _step2StreamReader = step2StreamReader;
            _step2Decoder = step2Decoder;
            _metadataAggregator = metadataAggregator;
            _sifStreamReader = sifStreamReader;
            _smStreamReader = smStreamReader;
            _sifSmMetadataChanger = sifSmMetadataChanger;
            _heuristicTester = heuristicTester;
            _ddr573AudioKeyProvider = ddr573AudioKeyProvider;
            _ddr573AudioDecrypter = ddr573AudioDecrypter;
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
                        var globalOffset = Args.Options.ContainsKey("-offset")
                            ? BigRationalParser.ParseString(Args.Options["-offset"].FirstOrDefault() ?? "0")
                            : BigRational.Zero;

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
                                [ChartTag.MusicTag] = aggregatedInfo[StringData.Music] ?? $"{title}.ogg",
                                [ChartTag.PreviewTag] = aggregatedInfo[StringData.Music] ?? $"{title}-preview.ogg",
                                [ChartTag.OffsetTag] = $"{(decimal) (-aggregatedInfo[NumericData.LinearOffset] + globalOffset)}"
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

        public ITask CreateDecrypt573Audio()
        {
            return Build("Decrypt 573 Audio", task =>
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
                        var encoded = inFile.ReadAllBytes();
                        var key = _ddr573AudioKeyProvider.Get(encoded);
                        if (key == null)
                        {
                            task.Message = $"Can't find key for {inputFile.Name}";
                            continue;
                        }
                        var decoded = (key.Length == 1)
                            ? _ddr573AudioDecrypter.DecryptOld(encoded, key[0])
                            : _ddr573AudioDecrypter.DecryptNew(encoded, key);

                        using (var outFile = OpenWriteSingle(task, inputFile, i => $"{i}.mp3"))
                        {
                            decoded.WriteAllBytes(outFile);
                            outFile.Flush();
                        }
                    }
                }

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
                        var aggregatedInfo = _metadataAggregator.Aggregate(charts);
                        var title = aggregatedInfo[StringData.Title] ?? Path.GetFileNameWithoutExtension(inputFile.Name);
                        var globalOffset = Args.Options.ContainsKey("-offset")
                            ? BigRationalParser.ParseString(Args.Options["-offset"].FirstOrDefault() ?? "0")
                            : BigRational.Zero;
                        var encoded = _smEncoder.Encode(new ChartSet
                        {
                            Metadata = new Metadata
                            {
                                [StringData.Title] = title,
                                [StringData.Subtitle] = aggregatedInfo[StringData.Subtitle],
                                [StringData.Artist] = aggregatedInfo[StringData.Artist],
                                [ChartTag.MusicTag] = aggregatedInfo[StringData.Music] ?? $"{title}.ogg",
                                [ChartTag.PreviewTag] = aggregatedInfo[StringData.Music] ?? $"{title}-preview.ogg",
                                [ChartTag.OffsetTag] = $"{(decimal) (-aggregatedInfo[NumericData.LinearOffset] + globalOffset)}"
                            },
                            Charts = charts
                        });

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
                        var sif = _sifStreamReader.Read(inFile, inFile.Length);
                        var name = Path.GetFileNameWithoutExtension(inputFile.Name);

                        if (!sif.KeyValues.ContainsKey("dir"))
                            sif.KeyValues["dir"] = name;
                        _sifSmMetadataChanger.Apply(sm, sif);

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
                            ? _ddr573ImageStreamReader.Read(fileStreams[0], (int) fileStreams[0].Length)
                            : _ddr573ImageStreamReader.Read(fileStreams[0], (int) fileStreams[0].Length, fileStreams[1],
                                (int) fileStreams[1].Length);
                    }
                    finally
                    {
                        foreach (var fileStream in fileStreams)
                            fileStream?.Dispose();
                    }

                    var files = _ddr573ImageDecoder.Decode(image);
                    var fileIndex = 0;
                    ParallelProgress(task, files, file =>
                    {
                        task.Progress = fileIndex / (float) files.Count;
                        var outFileName = $"{file.Module:X4}{file.Offset:X7}.bin";
                        task.Message = $"Writing {outFileName}";

                        var extension = (_heuristicTester.Match(file.Data.Span).FirstOrDefault()?.Heuristic.FileExtension ?? "bin")
                            .ToLowerInvariant();

                        using (var stream =
                            OpenWriteMulti(task, gameImage, _ => $"{file.Module:X4}{file.Offset:X7}.{extension}"))
                        {
                            var writer = new BinaryWriter(stream);
                            writer.Write(file.Data.ToArray());
                            stream.Flush();
                        }
                    });

                    task.Message = "Finished.";
                    return true;
                });
        }
    }
}