using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Infrastructure;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Wav.Converters;

namespace RhythmCodex.Wav.Integration
{
    public class WavDecodeIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [Explicit]
        [TestCase("msadpcm")]
        public void TestMicrosoftAdpcm(string name)
        {
            var decoder = Resolve<IWavDecoder>();
            var encoder = Resolve<IRiffPcm16SoundEncoder>();
            var writer = Resolve<IRiffStreamWriter>();
            
            var data = GetArchiveResource($"Wav.{name}.zip")
                .First()
                .Value;

            var sound = decoder.Decode(new ReadOnlyMemoryStream(data));
            var encoded = encoder.Encode(sound);
            var outfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "wav");
            if (!Directory.Exists(outfolder))
                Directory.CreateDirectory(outfolder);

            using var outStream = new MemoryStream();
            writer.Write(outStream, encoded);
            outStream.Flush();
            File.WriteAllBytes(Path.Combine(outfolder, $"{name}.wav"), outStream.ToArray());
        }
    }
}