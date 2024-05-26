using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClientCommon;
using RhythmCodex.Beatmania.Converters;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Beatmania.Streamers;
using RhythmCodex.Bms.Converters;
using RhythmCodex.Bms.Streamers;
using RhythmCodex.Charting.Models;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Djmain;
using RhythmCodex.Djmain.Converters;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Djmain.Streamers;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Providers;
using RhythmCodex.Statistics;
using RhythmCodex.Wav.Converters;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Cli.Orchestration;

[Service(singleInstance: false)]
public class BeatmaniaTaskBuilder(
    IFileSystem fileSystem,
    ILogger logger,
    IBeatmaniaPcAudioStreamReader beatmaniaPcAudioStreamReader,
    IRiffPcm16SoundEncoder riffPcm16SoundEncoder,
    IRiffStreamWriter riffStreamWriter,
    IAudioDsp audioDsp,
    IBeatmaniaPc1StreamReader beatmaniaPc1StreamReader,
    IBeatmaniaPc1ChartDecoder beatmaniaPc1ChartDecoder,
    IBmsEncoder bmsEncoder,
    IBmsStreamWriter bmsStreamWriter,
    IDjmainDecoder djmainDecoder,
    IDjmainChunkStreamReader djmainChunkStreamReader,
    IUsedSamplesCounter usedSamplesCounter,
    IBeatmaniaPcAudioDecoder beatmaniaPcAudioDecoder,
    IEncryptedBeatmaniaPcAudioStreamReader encryptedBeatmaniaPcAudioStreamReader,
    IResamplerProvider resamplerProvider,
    IDjmainChartEventStreamWriter djmainChartEventStreamWriter,
    IChartRenderer chartRenderer)
    : TaskBuilderBase<BeatmaniaTaskBuilder>(fileSystem, logger)
{
    private bool EnableExportingCharts => !Args.Options.ContainsKey("+nocharts");
    private bool EnableExportingSounds => !Args.Options.ContainsKey("+noaudio");
    private bool EnableExportingBms => !Args.Options.ContainsKey("+noconvert");
    private bool EnableExportingRaw => Args.Options.ContainsKey("+raw");

    public ITask CreateDecode1()
    {
        return Build("Decode 1", task =>
        {
            var rate = new BigRational(1000, 1);
            var files = GetInputFiles(task);
            if (!files.Any())
            {
                task.Message = "No input files.";
                return false;
            }

            if (Args.Options.TryGetValue("rate", out var option))
            {
                rate = BigRationalParser.ParseString(option.Last())
                       ?? throw new RhythmCodexException($"Invalid rate.");
            }

            ParallelProgress(task, files, file =>
            {
                using var stream = OpenRead(task, file);
                var charts = beatmaniaPc1StreamReader.Read(stream, stream.Length).ToList();
                var decoded = charts.Select(c =>
                {
                    var newChart = beatmaniaPc1ChartDecoder.Decode(c.Data, rate);
                    newChart[NumericData.Id] = c.Index;

                    switch (c.Index)
                    {
                        case 0:
                        case 6:
                        {
                            newChart[NumericData.Difficulty] = BeatmaniaDifficultyConstants.NormalId;
                            break;
                        }
                        case 1:
                        case 7:
                        {
                            newChart[NumericData.Difficulty] = BeatmaniaDifficultyConstants.LightId;
                            break;
                        }
                        case 2:
                        case 8:
                        {
                            newChart[NumericData.Difficulty] = BeatmaniaDifficultyConstants.AnotherId;
                            break;
                        }
                        case 3:
                        case 9:
                        {
                            newChart[NumericData.Difficulty] = BeatmaniaDifficultyConstants.BeginnerId;
                            break;
                        }
                    }

                    newChart[StringData.Title] = Path.GetFileNameWithoutExtension(file.Name);
                    return newChart;
                }).ToList();

                if (!EnableExportingCharts)
                    return;

                foreach (var chart in decoded)
                {
                    chart.PopulateMetricOffsets();
                    var encoded = bmsEncoder.Encode(chart);
                    using var outStream =
                        OpenWriteMulti(task, file,
                            _ => $"{Alphabet.EncodeNumeric((int)chart[NumericData.Id], 2)}.bme");
                    bmsStreamWriter.Write(outStream, encoded);
                }
            });

            return true;
        });
    }

    public ITask CreateExtract2dx()
    {
        return Build("Extract 2DX", task =>
        {
            var files = GetInputFiles(task);
            if (!files.Any())
            {
                task.Message = "No input files.";
                return false;
            }

            ParallelProgress(task, files, file =>
            {
                using var stream = OpenRead(task, file);
                var decrypted = encryptedBeatmaniaPcAudioStreamReader.Decrypt(stream, stream.Length);
                var sounds = beatmaniaPcAudioStreamReader.Read(new ReadOnlyMemoryStream(decrypted), decrypted.Length);
                var index = 1;

                if (EnableExportingSounds)
                {
                    foreach (var sound in sounds)
                    {
                        var decoded = beatmaniaPcAudioDecoder.Decode(sound);
                        var outSound = audioDsp.ApplyEffects(decoded);
                        using (var outStream =
                               OpenWriteMulti(task, file, _ => $"{Alphabet.EncodeAlphanumeric(index, 4)}.wav"))
                        {
                            var encoded = riffPcm16SoundEncoder.Encode(outSound);
                            riffStreamWriter.Write(outStream, encoded);
                        }

                        index++;
                    }
                }
            });

            return true;
        });
    }

    public ITask CreateDecodeDjmainHdd()
    {
        return Build("Extract DJMAIN HDD", task =>
        {
            var files = GetInputFiles(task);
            if (!files.Any())
            {
                task.Message = "No input files.";
                return false;
            }

            ParallelProgress(task, files, file =>
            {
                var options = new DjmainDecodeOptions
                {
                    DisableAudio = !EnableExportingSounds,
                    DoNotConsolidateSamples = false
                };

                using var stream = OpenRead(task, file);
                long offset = 0;
                var chunks = djmainChunkStreamReader.Read(stream);
                foreach (var chunk in chunks)
                {
                    var chunkPath = $"{Alphabet.EncodeNumeric(chunk.Id, 4)}";
                    var decoded = djmainDecoder.Decode(chunk, options);
                    ExportKeysoundedChart(task, file, chunkPath, $"{Alphabet.EncodeNumeric(chunk.Id, 4)}",
                        decoded.Charts, decoded.Samples);

                    if (EnableExportingRaw)
                    {
                        foreach (var (key, value) in decoded.RawCharts)
                        {
                            using var rawChartStream = OpenWriteMulti(task, file,
                                _ => Path.Combine(chunkPath, $"{Alphabet.EncodeNumeric(key, 2)}.cs5"));
                            djmainChartEventStreamWriter.Write(rawChartStream, value);
                            rawChartStream.Flush();
                        }
                    }

                    if (file.Length != null)
                    {
                        offset += DjmainConstants.ChunkSize;
                        task.Progress = offset / (float)file.Length;
                    }
                }
            });

            return true;
        });
    }

    public ITask CreateRenderDjmainGst()
    {
        return Build("Render DJMAIN GST", task =>
        {
            var files = GetInputFiles(task);
            if (!files.Any())
            {
                task.Message = "No input files.";
                return false;
            }

            ParallelProgress(task, files, file =>
            {
                var options = new DjmainDecodeOptions
                {
                    DoNotConsolidateSamples = true,
                    DisableAudio = false
                };

                var renderOptions = new ChartRendererOptions();

                using var stream = OpenRead(task, file);
                long offset = 0;
                var chunks = djmainChunkStreamReader.Read(stream);
                foreach (var chunk in chunks)
                {
                    var chunkPath = $"{Alphabet.EncodeNumeric(chunk.Id, 4)}";
                    var decoded = djmainDecoder.Decode(chunk, options);

                    foreach (var chart in decoded.Charts)
                    {
                        using var outStream = OpenWriteMulti(task, file,
                            _ => Path.Combine(chunkPath,
                                $"{Alphabet.EncodeNumeric((int)chart[NumericData.Id], 2)}.render.wav"));
                        var rendered = chartRenderer.Render(chart.Events, decoded.Samples, renderOptions);
                        var normalized = audioDsp.Normalize(rendered, 1.0f, true);
                        var encoded = riffPcm16SoundEncoder.Encode(normalized);
                        riffStreamWriter.Write(outStream, encoded);
                        outStream.Flush();
                    }

                    if (file.Length != null)
                    {
                        offset += DjmainConstants.ChunkSize;
                        task.Progress = offset / (float)file.Length;
                    }
                }
            });

            return true;
        });
    }

    protected void ExportKeysoundedChart(BuiltTask task, InputFile file, string path, string id,
        IEnumerable<Chart> charts, IEnumerable<Sound?> sounds)
    {
        var chartList = charts.ToList();
        var soundList = sounds.ToList();

        var usedSamples = chartList
            .SelectMany(chart => usedSamplesCounter.GetUsedSamples(chart.Events))
            .Distinct()
            .ToArray();

        if (EnableExportingSounds)
        {
            var matchingUsedSamples = soundList
                .Where(s => usedSamples.Contains((int)s[NumericData.Id]));
            
            foreach (var sound in matchingUsedSamples)
            {
                var outSound = audioDsp
                    .ApplyEffects(audioDsp.ApplyResampling(sound, resamplerProvider.GetBest(), 44100));

                using var outStream =
                    OpenWriteMulti(task, file,
                        _ => Path.Combine(path,
                            $"{Alphabet.EncodeAlphanumeric((int)sound![NumericData.Id]!, 4)}.wav"));
                var encoded = riffPcm16SoundEncoder.Encode(outSound);
                riffStreamWriter.Write(outStream, encoded);
            }
        }

        if (EnableExportingCharts)
        {
            foreach (var chart in chartList)
            {
                chart.PopulateMetricOffsets();
                chart[StringData.Title] = id;
                var encoded = bmsEncoder.Encode(chart);
                using var outStream =
                    OpenWriteMulti(task, file,
                        _ => Path.Combine(path,
                            $"{Alphabet.EncodeNumeric((int)chart[NumericData.Id], 2)}.bme"));
                bmsStreamWriter.Write(outStream, encoded);
            }
        }
    }
}