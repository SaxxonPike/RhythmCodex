using System.Collections.Concurrent;
using JetBrains.Annotations;
using RhythmCodex.Charts.Bms.Converters;
using RhythmCodex.Charts.Bmson.Converters;
using RhythmCodex.Charts.Bmson.Streamers;
using RhythmCodex.Charts.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Resampler.Providers;
using RhythmCodex.Sounds.Riff.Converters;
using RhythmCodex.Sounds.Riff.Streamers;

namespace RhythmCodex;

[PublicAPI]
public static class TestHelper
{
    extension(IResolver resolver)
    {
        public void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public void WriteSound(Sound? decoded, string outFileName, float gain = 1)
        {
            if (!resolver.OutputFileFilter(outFileName))
                return;

            var encoder = resolver.Resolve<IRiffPcm16SoundEncoder>();
            var writer = resolver.Resolve<IRiffStreamWriter>();
            var dsp = resolver.Resolve<IAudioDsp>();

            var outPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), outFileName);

            resolver.CreateDirectory(Path.GetDirectoryName(outPath)!);

            var rendered = dsp.ApplyEffects(decoded);
            
            rendered = dsp.ApplyEffects(rendered, new Metadata
            {
                [NumericData.Volume] = gain
            });

            var encoded = encoder.Encode(rendered);

            using var outStream = new MemoryStream();
            writer.Write(outStream, encoded);
            outStream.Flush();
            File.WriteAllBytes(outPath, outStream.ToArray());
        }

        public void WriteFile(byte[] data, string outFileName)
        {
            if (!resolver.OutputFileFilter(outFileName))
                return;

            resolver.WriteFile(data.AsSpan(), outFileName);
        }

        public void WriteFile(ReadOnlyMemory<byte> data, string outFileName)
        {
            if (!resolver.OutputFileFilter(outFileName))
                return;

            resolver.WriteFile(data.Span, outFileName);
        }

        public void WriteFile(ReadOnlySpan<byte> data, string outFileName)
        {
            if (!resolver.OutputFileFilter(outFileName))
                return;

            var outPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), outFileName);
            resolver.CreateDirectory(Path.GetDirectoryName(outPath)!);
            using var stream = File.OpenWrite(outPath);
            stream.Write(data);
            stream.Flush();
        }

        public Stream OpenWrite(string outFileName)
        {
            if (!resolver.OutputFileFilter(outFileName))
                return null!;

            var outPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), outFileName);
            resolver.CreateDirectory(Path.GetDirectoryName(outPath)!);
            return File.Open(outPath, FileMode.Create, FileAccess.ReadWrite);
        }

        public Stream OpenRead(string inFileName)
        {
            var inPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), inFileName);
            return File.Exists(inPath)
                ? File.Open(inPath, FileMode.Open, FileAccess.ReadWrite)
                : null;
        }

        public void Delete(string fileName)
        {
            if (!resolver.OutputFileFilter(fileName))
                return;

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            File.Delete(path);
        }

        public void WriteSet(IEnumerable<Chart> charts, IEnumerable<Sound> sounds,
            string outPath, string title, BmsChartType chartType)
        {
            // var bmsWriter = resolver.Resolve<IBmsStreamWriter>();
            // var bmsEncoder = resolver.Resolve<IBmsEncoder>();

            var bmsonWriter = resolver.Resolve<IBmsonStreamWriter>();
            var bmsonEncoder = resolver.Resolve<IBmsonChartConverter>();

            var resampler = resolver.Resolve<IResamplerProvider>().GetBest();

            var soundList = sounds.ToList();
            var soundHashFileMap = new ConcurrentDictionary<int, string>();
            var soundMap = new Dictionary<(int SampleMap, int Index), int>();

            foreach (var sound in soundList.Where(s => s.Samples.Count != 0).AsParallel())
            {
                // Perform resampling.

                // foreach (var sample in sound.Samples)
                // {
                //     var sourceRate = (float)(double)(sample[NumericData.Rate] ?? sound[NumericData.Rate])!;
                //     if (sourceRate < 4000)
                //         continue;
                //
                //     if (sourceRate < 44100)
                //     {
                //         var resampled = resampler.Resample(sample.Data.Span, sourceRate, 44100);
                //         sample.Data = resampled;
                //         sample[NumericData.Rate] = 44100;
                //     }
                // }
                //
                // sound[NumericData.Rate] = sound.Samples
                //     .Select(s => s[NumericData.Rate])
                //     .FirstOrDefault(r => r != null);

                soundHashFileMap.AddOrUpdate(sound.CalculateSampleHash() ^ sound.CalculateSourceVolumePanHash(),
                    h =>
                    {
                        var fileNameId = (int)(sound[NumericData.SampleMap] ?? 0) * 1000 +
                                         (int)sound[NumericData.Id]!;
                        var fileName = $"{Alphabet.EncodeNumeric(fileNameId, 4)}.wav";

                        resolver.WriteSound(sound, Path.Combine(outPath, fileName), 0.7f);
                        soundMap.Add(((int)(sound[NumericData.SampleMap] ?? 0), (int)sound[NumericData.Id]!), h);
                        return fileName;
                    },
                    (h, val) =>
                    {
                        soundMap.Add(((int)(sound[NumericData.SampleMap] ?? 0), (int)sound[NumericData.Id]!), h);
                        return val;
                    }
                );
            }

            foreach (var chart in charts.AsParallel())
            {
                var usedCols = chart.Events
                    .Where(ev => ev[FlagData.Note] == true)
                    .Select(ev => (Column: ev[NumericData.Column], Player: ev[NumericData.Player]))
                    .Where(ev => ev is { Column: not null, Player: not null })
                    .Distinct()
                    .ToList();

                var playerCount = usedCols.Select(x => x.Player).Distinct().Count();
                var colCount = usedCols.Select(x => x.Column).Distinct().Count();

                var modeHint = chart[StringData.Type] ?? (chartType, playerCount, colCount) switch
                {
                    (BmsChartType.Beatmania, 2, > 6) => "beat-14k",
                    (BmsChartType.Beatmania, 1, > 0) => "beat-5k",
                    (BmsChartType.Beatmania, 2, > 0) => "beat-10k",
                    (BmsChartType.Beatmania, _, > 0) => "beat-7k",
                    (BmsChartType.Popn, _, > 5) => "popn-9k",
                    (BmsChartType.Popn, _, > 0) => "popn-5k",
                    _ => null
                };

                if (chart.Events.Any(e => e[FlagData.Note] == true && e[FlagData.FootPedal] == true))
                    modeHint = $"{modeHint}-fp";
                
                chart.PopulateMetricOffsets(normalizeMeasures: false);
                chart[StringData.Title] = title;

                // var chartPath = Path.Combine(outPath,
                //     $"@{Alphabet.EncodeAlphanumeric((int)(chart[NumericData.SampleMap] ?? 0), 1)}" +
                //     $"{Alphabet.EncodeHex((int)chart[NumericData.Id]!, 3)}.bms");

                var chartPath = Path.Combine(outPath,
                    $"@{Alphabet.EncodeAlphanumeric((int)(chart[NumericData.SampleMap] ?? 0), 1)}" +
                    $"{Alphabet.EncodeHex((int)chart[NumericData.Id]!, 3)}.bmson");

                if (!resolver.OutputFileFilter(chartPath))
                    continue;

                using var outStream = resolver.OpenWrite(chartPath);

                var exported = bmsonEncoder.Export(chart, new BmsonEncoderOptions
                {
                    WavNameTransformer = i =>
                    {
                        if (!soundMap.TryGetValue(((int)(chart[NumericData.SampleMap] ?? 0), i), out var sm) ||
                            !soundHashFileMap.TryGetValue(sm, out var fileName))
                            return null;

                        return fileName;
                    },
                    ChartType = chartType,
                    ModeHint = modeHint
                });
                
                bmsonWriter.Write(outStream, exported);

                // bmsWriter.Write(outStream, bmsEncoder.Encode(chart, new BmsEncoderOptions
                // {
                //     WavNameTransformer = i =>
                //     {
                //         if (!soundMap.TryGetValue(((int)(chart[NumericData.SampleMap] ?? 0), i), out var sm) ||
                //             !soundHashFileMap.TryGetValue(sm, out var fileName))
                //             return null;
                //
                //         return fileName;
                //     },
                //     ChartType = chartType
                // }));

                outStream.Flush();
            }
        }
    }
}