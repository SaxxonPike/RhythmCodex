using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Attributes;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Vag.Converters;
using RhythmCodex.Vag.Streamers;
using RhythmCodex.Xa.Converters;
using RhythmCodex.Xa.Models;

namespace RhythmCodex.Xa.Integration
{
    [TestFixture]
    public class XaDecodeIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [Explicit]
        public void Test_Xa()
        {
            var data = GetArchiveResource($"Xa.xa.zip")
                .First()
                .Value;

            var decoder = Resolve<IXaDecoder>();
            var encoder = Resolve<IRiffPcm16SoundEncoder>();
            var writer = Resolve<IRiffStreamWriter>();
            var xa = new XaChunk
            {
                Data = data,
                Channels = 4,
                Interleave = 0x800
            };

            var decoded = decoder.Decode(xa);
            var index = 0;

            foreach (var sound in decoded)
            {
                sound[NumericData.Rate] = 37800;
                var encoded = encoder.Encode(sound);
                using (var outStream = new MemoryStream())
                {
                    writer.Write(outStream, encoded);
                    outStream.Flush();
                    File.WriteAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"xa-{index}.wav"), outStream.ToArray());
                    index++;
                }
            }
        }
        
    }
}