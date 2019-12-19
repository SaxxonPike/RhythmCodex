using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Beatmania.Converters;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Beatmania.Streamers;
using RhythmCodex.Bms.Converters;
using RhythmCodex.Bms.Streamers;
using RhythmCodex.Charting.Models;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Djmain.Converters;
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

namespace RhythmCodex.Cli.Orchestration
{
    [Service(singleInstance: false)]
    public class BeatmaniaTaskBuilder : TaskBuilderBase<BeatmaniaTaskBuilder>
    {
        private readonly IBeatmaniaPcAudioStreamReader _beatmaniaPcAudioStreamReader;
        private readonly IRiffPcm16SoundEncoder _riffPcm16SoundEncoder;
        private readonly IRiffStreamWriter _riffStreamWriter;
        private readonly IAudioDsp _audioDsp;
        private readonly IBeatmaniaPc1StreamReader _beatmaniaPc1StreamReader;
        private readonly IBeatmaniaPc1ChartDecoder _beatmaniaPc1ChartDecoder;
        private readonly IBmsEncoder _bmsEncoder;
        private readonly IBmsStreamWriter _bmsStreamWriter;
        private readonly IDjmainDecoder _djmainDecoder;
        private readonly IDjmainChunkStreamReader _djmainChunkStreamReader;
        private readonly IUsedSamplesCounter _usedSamplesCounter;
        private readonly IBeatmaniaPcAudioDecoder _beatmaniaPcAudioDecoder;
        private readonly IEncryptedBeatmaniaPcAudioStreamReader _encryptedBeatmaniaPcAudioStreamReader;
        private readonly IResamplerProvider _resamplerProvider;

        public BeatmaniaTaskBuilder(
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
            IResamplerProvider resamplerProvider
        )
            : base(fileSystem, logger)
        {
            _beatmaniaPcAudioStreamReader = beatmaniaPcAudioStreamReader;
            _riffPcm16SoundEncoder = riffPcm16SoundEncoder;
            _riffStreamWriter = riffStreamWriter;
            _audioDsp = audioDsp;
            _beatmaniaPc1StreamReader = beatmaniaPc1StreamReader;
            _beatmaniaPc1ChartDecoder = beatmaniaPc1ChartDecoder;
            _bmsEncoder = bmsEncoder;
            _bmsStreamWriter = bmsStreamWriter;
            _djmainDecoder = djmainDecoder;
            _djmainChunkStreamReader = djmainChunkStreamReader;
            _usedSamplesCounter = usedSamplesCounter;
            _beatmaniaPcAudioDecoder = beatmaniaPcAudioDecoder;
            _encryptedBeatmaniaPcAudioStreamReader = encryptedBeatmaniaPcAudioStreamReader;
            _resamplerProvider = resamplerProvider;
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
                        var charts = _beatmaniaPc1StreamReader.Read(stream, stream.Length).ToList();
                        var decoded = charts.Select(c =>
                        {
                            var newChart = _beatmaniaPc1ChartDecoder.Decode(c.Data, rate);
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
                        var decrypted = _encryptedBeatmaniaPcAudioStreamReader.Decrypt(stream, stream.Length);
                        var sounds = _beatmaniaPcAudioStreamReader.Read(new MemoryStream(decrypted), decrypted.Length);
                        var index = 1;
                        foreach (var sound in sounds)
                        {
                            var decoded = _beatmaniaPcAudioDecoder.Decode(sound);
                            var outSound = _audioDsp.ApplyEffects(decoded);
                            using (var outStream =
                                OpenWriteMulti(task, file, i => $"{Alphabet.EncodeAlphanumeric(index, 4)}.wav"))
                            {
                                var encoded = _riffPcm16SoundEncoder.Encode(outSound);
                                _riffStreamWriter.Write(outStream, encoded);
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
                var outSound = _audioDsp.ApplyEffects(_audioDsp.ApplyResampling(sound, _resamplerProvider.GetBest(), 44100));
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