using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using RhythmCodex.Bms.Converters;
using RhythmCodex.Bms.Streamers;
using RhythmCodex.Charting.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Providers;

namespace RhythmCodex
{
    public static class TestHelper
    {
        public static void CreateDirectory(this IResolver resolver, string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static void WriteSound(this IResolver resolver, ISound decoded, string outFileName)
        {
            var encoder = resolver.Resolve<IRiffPcm16SoundEncoder>();
            var writer = resolver.Resolve<IRiffStreamWriter>();
            var dsp = resolver.Resolve<IAudioDsp>();

            var outPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), outFileName);

            CreateDirectory(resolver, Path.GetDirectoryName(outPath));

            var encoded = encoder.Encode(dsp.ApplyEffects(decoded));

            using var outStream = new MemoryStream();
            writer.Write(outStream, encoded);
            outStream.Flush();
            File.WriteAllBytes(outPath, outStream.ToArray());
        }

        public static void WriteSet(this IResolver resolver, 
            IEnumerable<IChart> charts, IEnumerable<ISound> sounds, 
            string outPath, string title)
        {
            var bmsWriter = resolver.Resolve<IBmsStreamWriter>();
            var bmsEncoder = resolver.Resolve<IBmsEncoder>();
            var resampler = resolver.Resolve<IResamplerProvider>().GetBest();

            var soundList = sounds.AsList();

            foreach (var sound in soundList.Where(s => s.Samples.Any()).AsParallel())
            {
                foreach (var sample in sound.Samples)
                {
                    var sourceRate = (float)(double)(sample[NumericData.Rate] ?? sound[NumericData.Rate]);
                    var resampled = resampler.Resample(sample.Data, sourceRate, 44100);
                    sample.Data = resampled;
                    sample[NumericData.Rate] = 44100;
                }

                sound[NumericData.Rate] = 44100;
                WriteSound(resolver, sound, Path.Combine(outPath, $"{Alphabet.EncodeAlphanumeric((int)sound[NumericData.Id], 4)}.wav"));
            }

            foreach (var chart in charts.AsParallel())
            {
                chart.PopulateMetricOffsets();
                chart[StringData.Title] = title;
                using var outStream = OpenWrite(resolver, Path.Combine(outPath, $"@{Alphabet.EncodeHex((int)chart[NumericData.ByteOffset], 7)}.bms"));
                bmsWriter.Write(outStream, bmsEncoder.Encode(chart));
                outStream.Flush();
            }
        }

        public static void WriteFile(this IResolver resolver, byte[] data, string outFileName)
        {
            var outPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), outFileName);
            CreateDirectory(resolver, Path.GetDirectoryName(outPath));
            File.WriteAllBytes(outPath, data);
        }

        public static Stream OpenRead(this IResolver resolver, string inFileName)
        {
            var inPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), inFileName);
            return File.Exists(inPath) 
                ? File.Open(inPath, FileMode.Open, FileAccess.ReadWrite) 
                : null;
        }

        public static Stream OpenWrite(this IResolver resolver, string outFileName)
        {
            var outPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), outFileName);
            CreateDirectory(resolver, Path.GetDirectoryName(outPath));
            return File.Open(outPath, FileMode.Create, FileAccess.ReadWrite);
        }

        public static void Delete(this IResolver resolver, string fileName)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            File.Delete(path);
        }
    }
}