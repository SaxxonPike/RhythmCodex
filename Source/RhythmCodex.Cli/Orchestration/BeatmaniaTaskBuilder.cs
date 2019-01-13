using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Beatmania.Converters;
using RhythmCodex.Beatmania.Streamers;
using RhythmCodex.Bms.Converters;
using RhythmCodex.Bms.Streamers;
using RhythmCodex.Charting;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Djmain.Converters;
using RhythmCodex.Djmain.Streamers;
using RhythmCodex.Dsp;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Statistics;

namespace RhythmCodex.Cli.Orchestration
{
    [InstancePerDependency]
    public class BeatmaniaTaskBuilder : TaskBuilderBase<BeatmaniaTaskBuilder>
    {
        private readonly IBeatmaniaPcAudioStreamer _beatmaniaPcAudioStreamer;
        private readonly IRiffPcm16SoundEncoder _riffPcm16SoundEncoder;
        private readonly IRiffStreamWriter _riffStreamWriter;
        private readonly IAudioDsp _audioDsp;
        private readonly IBeatmaniaPc1Streamer _beatmaniaPc1Streamer;
        private readonly IBeatmaniaPc1ChartDecoder _beatmaniaPc1ChartDecoder;
        private readonly IBmsEncoder _bmsEncoder;
        private readonly IBmsStreamWriter _bmsStreamWriter;
        private readonly IDjmainDecoder _djmainDecoder;
        private readonly IDjmainChunkStreamReader _djmainChunkStreamReader;
        private readonly IUsedSamplesCounter _usedSamplesCounter;

        public BeatmaniaTaskBuilder(
            IFileSystem fileSystem,
            ILogger logger,
            IBeatmaniaPcAudioStreamer beatmaniaPcAudioStreamer,
            IRiffPcm16SoundEncoder riffPcm16SoundEncoder,
            IRiffStreamWriter riffStreamWriter,
            IAudioDsp audioDsp,
            IBeatmaniaPc1Streamer beatmaniaPc1Streamer,
            IBeatmaniaPc1ChartDecoder beatmaniaPc1ChartDecoder,
            IBmsEncoder bmsEncoder,
            IBmsStreamWriter bmsStreamWriter,
            IDjmainDecoder djmainDecoder,
            IDjmainChunkStreamReader djmainChunkStreamReader,
            IUsedSamplesCounter usedSamplesCounter
        )
            : base(fileSystem, logger)
        {
            _beatmaniaPcAudioStreamer = beatmaniaPcAudioStreamer;
            _riffPcm16SoundEncoder = riffPcm16SoundEncoder;
            _riffStreamWriter = riffStreamWriter;
            _audioDsp = audioDsp;
            _beatmaniaPc1Streamer = beatmaniaPc1Streamer;
            _beatmaniaPc1ChartDecoder = beatmaniaPc1ChartDecoder;
            _bmsEncoder = bmsEncoder;
            _bmsStreamWriter = bmsStreamWriter;
            _djmainDecoder = djmainDecoder;
            _djmainChunkStreamReader = djmainChunkStreamReader;
            _usedSamplesCounter = usedSamplesCounter;
        }

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

                if (Args.Options.ContainsKey("rate"))
                {
                    rate = BigRationalParser.ParseString(Args.Options["rate"].Last())
                           ?? throw new RhythmCodexException($"Invalid rate.");
                }

                ParallelProgress(task, files, file =>
                {
                    using (var stream = OpenRead(task, file))
                    {
                        var charts = _beatmaniaPc1Streamer.Read(stream, stream.Length).ToList();
                        var decoded = charts.Select(c =>
                        {
                            var newChart = _beatmaniaPc1ChartDecoder.Decode(c.Data, rate);
                            newChart[NumericData.Id] = c.Index;

                            switch (c.Index)
                            {
                                case 0:
                                case 6:
                                {
                                    newChart[NumericData.Difficulty] = 3;
                                    break;
                                }
                                case 1:
                                case 7:
                                {
                                    newChart[NumericData.Difficulty] = 2;
                                    break;
                                }
                                case 2:
                                case 8:
                                {
                                    newChart[NumericData.Difficulty] = 4;
                                    break;
                                }
                                case 3:
                                case 9:
                                {
                                    newChart[NumericData.Difficulty] = 1;
                                    break;
                                }
                            }

                            newChart[StringData.Title] = Path.GetFileNameWithoutExtension(file.Name);
                            return newChart;
                        }).ToList();

                        foreach (var chart in decoded)
                        {
                            chart.PopulateMetricOffsets();
                            var encoded = _bmsEncoder.Encode(chart);
                            using (var outStream =
                                OpenWriteMulti(task, file,
                                    i => $"{Alphabet.EncodeNumeric((int) chart[NumericData.Id], 2)}.bme"))
                            {
                                _bmsStreamWriter.Write(outStream, encoded);
                            }
                        }
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
                    using (var stream = OpenRead(task, file))
                    {
                        var decrypted = _beatmaniaPcAudioStreamer.Decrypt(stream, stream.Length);
                        using (var decryptedStream = new MemoryStream(decrypted))
                        {
                            var sounds = _beatmaniaPcAudioStreamer.Read(decryptedStream, stream.Length);
                            var index = 1;
                            foreach (var sound in sounds)
                            {
                                var outSound = _audioDsp.ApplyEffects(sound);
                                using (var outStream =
                                    OpenWriteMulti(task, file, i => $"{Alphabet.EncodeAlphanumeric(index, 4)}.wav"))
                                {
                                    var encoded = _riffPcm16SoundEncoder.Encode(outSound);
                                    _riffStreamWriter.Write(outStream, encoded);
                                }

                                index++;
                            }
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
                    using (var stream = OpenRead(task, file))
                    {
                        var chunks = _djmainChunkStreamReader.Read(stream);
                        foreach (var chunk in chunks)
                        {
                            var chunkPath = $"{Alphabet.EncodeNumeric(chunk.Id, 4)}";
                            var decoded = _djmainDecoder.Decode(chunk);
                            ExportKeysoundedChart(task, file, chunkPath, $"{Alphabet.EncodeNumeric(chunk.Id, 4)}",
                                decoded.Charts, decoded.Samples);
                        }
                    }
                });

                return true;
            });
        }

        protected void ExportKeysoundedChart(BuiltTask task, InputFile file, string path, string id,
            ICollection<IChart> charts, ICollection<ISound> sounds)
        {
            var usedSamples = charts
                .SelectMany(chart => _usedSamplesCounter.GetUsedSamples(chart.Events))
                .Distinct()
                .ToArray();
            
            foreach (var sound in sounds.Where(s => usedSamples.Contains((int)s[NumericData.Id])))
            {
                var outSound = _audioDsp.ApplyEffects(_audioDsp.ApplyResampling(sound, 44100));
                using (var outStream =
                    OpenWriteMulti(task, file,
                        i => Path.Combine(path,
                            $"{Alphabet.EncodeAlphanumeric((int) sound[NumericData.Id], 4)}.wav")))
                {
                    var encoded = _riffPcm16SoundEncoder.Encode(outSound);
                    _riffStreamWriter.Write(outStream, encoded);
                }
            }

            foreach (var chart in charts)
            {
                chart.PopulateMetricOffsets();
                chart[StringData.Title] = id;
                var encoded = _bmsEncoder.Encode(chart);
                using (var outStream =
                    OpenWriteMulti(task, file,
                        i => Path.Combine(path,
                            $"{Alphabet.EncodeNumeric((int) chart[NumericData.Id], 2)}.bme")))
                {
                    _bmsStreamWriter.Write(outStream, encoded);
                }
            }
        }
    }
}