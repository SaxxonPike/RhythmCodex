using System;
using System.IO;
using RhythmCodex.Dsp;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Sounds.Models;

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
            using (var outStream = new MemoryStream())
            {
                writer.Write(outStream, encoded);
                outStream.Flush();
                File.WriteAllBytes(outPath, outStream.ToArray());
            }
        }
    }
}