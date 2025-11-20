using System.Collections.Concurrent;
using JetBrains.Annotations;
using RhythmCodex.Charts.Bms.Converters;
using RhythmCodex.Charts.Bms.Streamers;
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

        public void WriteSound(Sound? decoded, string outFileName)
        {
            var encoder = resolver.Resolve<IRiffPcm16SoundEncoder>();
            var writer = resolver.Resolve<IRiffStreamWriter>();
            var dsp = resolver.Resolve<IAudioDsp>();

            var outPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), outFileName);

            CreateDirectory(resolver, Path.GetDirectoryName(outPath)!);

            var encoded = encoder.Encode(dsp.ApplyEffects(decoded));
            using var outStream = new MemoryStream();
            writer.Write(outStream, encoded);
            outStream.Flush();
            File.WriteAllBytes(outPath, outStream.ToArray());
        }

        public void WriteFile(byte[] data, string outFileName)
        {
            WriteFile(resolver, data.AsSpan(), outFileName);
        }

        public void WriteFile(ReadOnlyMemory<byte> data, string outFileName)
        {
            WriteFile(resolver, data.Span, outFileName);
        }

        public void WriteFile(ReadOnlySpan<byte> data, string outFileName)
        {
            var outPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), outFileName);
            CreateDirectory(resolver, Path.GetDirectoryName(outPath)!);
            using var stream = File.OpenWrite(outPath);
            stream.Write(data);
            stream.Flush();
        }

        public Stream OpenWrite(string outFileName)
        {
            var outPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), outFileName);
            CreateDirectory(resolver, Path.GetDirectoryName(outPath)!);
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
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            File.Delete(path);
        }

        public void WriteSet(IEnumerable<Chart> charts, IEnumerable<Sound> sounds,
            string outPath, string title)
        {
            var bmsWriter = resolver.Resolve<IBmsStreamWriter>();
            var bmsEncoder = resolver.Resolve<IBmsEncoder>();
            var resampler = resolver.Resolve<IResamplerProvider>().GetBest();

            var soundList = sounds.ToList();
            var soundHashFileMap = new ConcurrentDictionary<int, string>();
            var soundMap = new Dictionary<(int SampleMap, int Index), int>();

            foreach (var sound in soundList.Where(s => s.Samples.Count != 0).AsParallel())
            {
                foreach (var sample in sound.Samples)
                {
                    var sourceRate = (float)(double)(sample[NumericData.Rate] ?? sound[NumericData.Rate])!;
                    if (sourceRate < 4000)
                        continue;

                    var resampled = resampler.Resample(sample.Data.Span, sourceRate, 44100);
                    sample.Data = resampled;
                    sample[NumericData.Rate] = 44100;
                }

                sound[NumericData.Rate] = 44100;

                soundHashFileMap.AddOrUpdate(sound.CalculateSampleHash(),
                    h =>
                    {
                        var fileName = string.Format("{0}{1}.wav",
                            Alphabet.EncodeAlphanumeric((int)(sound[NumericData.SampleMap] ?? 0), 1),
                            Alphabet.EncodeAlphanumeric((int)sound[NumericData.Id]!, 3)
                        );

                        WriteSound(resolver, sound, Path.Combine(outPath, fileName));
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
                chart.PopulateMetricOffsets();
                chart[StringData.Title] = title;
                using var outStream = OpenWrite(resolver,
                    Path.Combine(outPath,
                        string.Format("@{0}{1}.bms",
                            Alphabet.EncodeAlphanumeric((int)(chart[NumericData.SampleMap] ?? 0), 1),
                            Alphabet.EncodeHex((int)chart[NumericData.Id]!, 3)
                        )));

                bmsWriter.Write(outStream, bmsEncoder.Encode(chart, new BmsEncoderOptions
                {
                    WavNameTransformer = i =>
                    {
                        if (!soundMap.TryGetValue(((int)(chart[NumericData.SampleMap] ?? 0), i), out var sm) ||
                            !soundHashFileMap.TryGetValue(sm, out var fileName))
                            return null;

                        return fileName;
                    }
                }));

                outStream.Flush();
            }
        }
    }
}