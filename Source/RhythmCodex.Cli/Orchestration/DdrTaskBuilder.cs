using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClientCommon;
using RhythmCodex.Archs.S573.Converters;
using RhythmCodex.Archs.S573.Providers;
using RhythmCodex.Charts.Ssq.Converters;
using RhythmCodex.Charts.Ssq.Streamers;
using RhythmCodex.Charts.Step1.Converters;
using RhythmCodex.Charts.Step1.Streamers;
using RhythmCodex.Charts.Step2.Converters;
using RhythmCodex.Charts.Step2.Streamers;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Games.Ddr.S573.Converters;
using RhythmCodex.Games.Ddr.S573.Models;
using RhythmCodex.Games.Ddr.S573.Processors;
using RhythmCodex.Games.Ddr.Streamers;
using RhythmCodex.Games.Stepmania;
using RhythmCodex.Games.Stepmania.Converters;
using RhythmCodex.Games.Stepmania.Model;
using RhythmCodex.Games.Stepmania.Streamers;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Metadatas.Sif.Converters;
using RhythmCodex.Metadatas.Sif.Streamers;

namespace RhythmCodex.Cli.Orchestration;

[Service(singleInstance: false)]
public class DdrTaskBuilder(
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
    IDigital573AudioKeyProvider digital573AudioKeyProvider,
    IDigital573AudioDecrypter digital573AudioDecrypter,
    IDdr573AudioNameFinder ddr573AudioNameFinder,
    IDdr573ImageFileNameHasher ddr573ImageFileNameHasher)
    : TaskBuilderBase<DdrTaskBuilder>(fileSystem, logger)
{
    public ITask CreateDecodeSsq()
    {
        return Build("Decode SSQ", task =>
        {
            var files = GetInputFiles(task);
            if (files.Length == 0)
            {
                task.Message = "No input files.";
                return false;
            }

            ParallelProgress(task, files, file =>
            {
                using var inFile = OpenRead(task, file);
                var chunks = ssqStreamReader.Read(inFile);
                var charts = ssqDecoder.Decode(chunks);
                var aggregatedInfo = metadataAggregator.Aggregate(charts);
                var title = Path.GetFileNameWithoutExtension(file.Name);
                var globalOffset = Args.Options.TryGetValue("-offset", out var option)
                    ? BigRationalParser.ParseString(option.FirstOrDefault() ?? "0") ?? BigRational.Zero
                    : BigRational.Zero;

                // This is a temporary hack to make building sets easier for right now
                // TODO: make this optional via command line switch
                if (title.EndsWith("_all", StringComparison.InvariantCultureIgnoreCase))
                    title = title[..^4];

                var encoded = smEncoder.Encode(new ChartSet
                {
                    Metadata = new Metadata
                    {
                        [StringData.Title] = aggregatedInfo[StringData.Title] ?? title,
                        [StringData.Subtitle] = aggregatedInfo[StringData.Subtitle],
                        [StringData.Artist] = aggregatedInfo[StringData.Artist],
                        [ChartTag.MusicTag] = aggregatedInfo[StringData.Music] ?? $"{title}.ogg",
                        [ChartTag.PreviewTag] = aggregatedInfo[StringData.Music] ?? $"{title}-preview.ogg",
                        [ChartTag.OffsetTag] = $"{(decimal) (-(aggregatedInfo[NumericData.LinearOffset] ?? 0) + globalOffset)}"
                    },
                    Charts = charts
                });

                using var outFile = OpenWriteSingle(task, file, i => $"{i}.sm");
                smStreamWriter.Write(outFile, encoded);
                outFile.Flush();
            });

            return true;
        });
    }

    public ITask CreateDecrypt573Audio()
    {
        return Build("Decrypt 573 Audio", task =>
        {
            var inputFiles = GetInputFiles(task);
            if (inputFiles.Length == 0)
            {
                task.Message = "No input files.";
                return false;
            }

            foreach (var inputFile in inputFiles)
            {
                using var inFile = OpenRead(task, inputFile);
                var encoded = inFile.ReadAllBytes();
                var key = digital573AudioKeyProvider.Get(encoded.Span);
                if (key == null)
                {
                    task.Message = $"Can't find key for {inputFile.Name}";
                    continue;
                }
                var decoded = key.Values.Count == 1
                    ? digital573AudioDecrypter.DecryptOld(encoded.Span, key.Values[0])
                    : digital573AudioDecrypter.DecryptNew(encoded.Span, key);

                using var outFile = OpenWriteSingle(task, inputFile, i => Args.Options.ContainsKey("+name")
                    ? ddr573AudioNameFinder.GetPath(i!)
                    : $"{i}.mp3");
                decoded.Data.WriteAllBytes(outFile);
                outFile.Flush();
            }

            return true;
        });
    }

    public ITask CreateDecodeStep1()
    {
        return Build("Decode STEP", task =>
        {
            var inputFiles = GetInputFiles(task);
            if (inputFiles.Length == 0)
            {
                task.Message = "No input files.";
                return false;
            }

            foreach (var inputFile in inputFiles)
            {
                using var inFile = OpenRead(task, inputFile);
                var chunks = step1StreamReader.Read(inFile);
                var charts = step1Decoder.Decode(chunks);
                var aggregatedInfo = metadataAggregator.Aggregate(charts);
                var title = aggregatedInfo[StringData.Title] ?? Path.GetFileNameWithoutExtension(inputFile.Name);
                var globalOffset = Args.Options.TryGetValue("-offset", out var option)
                    ? BigRationalParser.ParseString(option.FirstOrDefault() ?? "0") ?? BigRational.Zero
                    : BigRational.Zero;
                var encoded = smEncoder.Encode(new ChartSet
                {
                    Metadata = new Metadata
                    {
                        [StringData.Title] = title,
                        [StringData.Subtitle] = aggregatedInfo[StringData.Subtitle],
                        [StringData.Artist] = aggregatedInfo[StringData.Artist],
                        [ChartTag.MusicTag] = aggregatedInfo[StringData.Music] ?? $"{title}.ogg",
                        [ChartTag.PreviewTag] = aggregatedInfo[StringData.Music] ?? $"{title}-preview.ogg",
                        [ChartTag.OffsetTag] = $"{(decimal) (-(aggregatedInfo[NumericData.LinearOffset] ?? 0) + globalOffset)}"
                    },
                    Charts = charts
                });

                using var outFile = OpenWriteSingle(task, inputFile, i => $"{i}.sm");
                smStreamWriter.Write(outFile, encoded);
                outFile.Flush();
            }

            return true;
        });
    }

    public ITask CreateDecodeStep2()
    {
        return Build("Decode STEP2", task =>
        {
            var inputFiles = GetInputFiles(task);
            if (inputFiles.Length == 0)
            {
                task.Message = "No input files.";
                return false;
            }

            foreach (var inputFile in inputFiles)
            {
                using var inFile = OpenRead(task, inputFile);
                var chunks = step2StreamReader.Read(inFile, (int) inFile.Length);
                var chart = step2Decoder.Decode(chunks);
                var encoded = smEncoder.Encode(new ChartSet
                    {Metadata = new Metadata(), Charts = [chart]});

                using var outFile = OpenWriteSingle(task, inputFile, i => $"{i}.sm");
                smStreamWriter.Write(outFile, encoded);
                outFile.Flush();
            }

            return true;
        });
    }

    public ITask CreateApplySif()
    {
        return Build("Apply SIF metadata", task =>
        {
            var inputFiles = GetInputFiles(task);
            if (inputFiles.Length == 0)
            {
                task.Message = "No input files.";
                return false;
            }

            foreach (var inputFile in inputFiles)
            {
                using var inFile = OpenRead(task, inputFile);
                using var smFile = OpenRelatedRead(inputFile, i => $"{i}_all.sm");
                var sm = smStreamReader.Read(smFile).ToList();
                var sif = sifStreamReader.Read(inFile, inFile.Length);
                var name = Path.GetFileNameWithoutExtension(inputFile.Name);

                sif.KeyValues.TryAdd("dir", name);
                sifSmMetadataChanger.Apply(sm, sif);

                smFile.Dispose();
                using var outStream = OpenWriteSingle(task, inputFile, _ => $"{name}_all.sm");
                smStreamWriter.Write(outStream, sm);
                outStream.Flush();
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
                        ? ddr573ImageStreamReader.Read(fileStreams[0], (int) fileStreams[0].Length)
                        : ddr573ImageStreamReader.Read(fileStreams[0], (int) fileStreams[0].Length, fileStreams[1],
                            (int) fileStreams[1].Length);
                }
                finally
                {
                    foreach (var fileStream in fileStreams)
                        fileStream?.Dispose();
                }

                var files = ddr573ImageDecoder.Decode(image, Args.Options.TryGetValue("k", out var option) ? option.Last() : null);
                var fileNames = ddr573ImageFileNameHasher.Reverse(files.Select(f => f.Id).ToArray());
                var fileIndex = 0;
                ParallelProgress(task, files, file =>
                {
                    task.Progress = fileIndex / (float) files.Count;
                    var extension = (heuristicTester.Match(file.Data).FirstOrDefault()?.Heuristic.FileExtension ?? "bin")
                        .ToLowerInvariant();
                    string outFileName;
                    if (Args.Options.ContainsKey("+name"))
                    {
                        outFileName = fileNames.TryGetValue(file.Id, out var id)
                            ? Path.Combine("./", id)
                            : $"{file.Module:X4}{file.Offset:X7}.{extension}";
                    }
                    else
                    {
                        outFileName = $"{file.Module:X4}{file.Offset:X7}.{extension}";
                    }
                    task.Message = $"Writing {outFileName}";

                    using var stream = OpenWriteMulti(task, gameImage, _ => outFileName);
                    stream.Write(file.Data.Span);
                    stream.Flush();
                });

                task.Message = "Finished.";
                return true;
            });
    }
}