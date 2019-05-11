using System;
using System.IO;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex
{
    public static class TestHelper
    {
        public static void WriteSound(this IResolver resolver, ISound decoded, string outFileName)
        {
            var encoder = resolver.Resolve<IRiffPcm16SoundEncoder>();
            var writer = resolver.Resolve<IRiffStreamWriter>();

            var encoded = encoder.Encode(decoded);
            using (var outStream = new MemoryStream())
            {
                writer.Write(outStream, encoded);
                outStream.Flush();
                File.WriteAllBytes(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), outFileName),
                    outStream.ToArray());
            }
        }
    }
}