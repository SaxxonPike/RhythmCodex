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
    public class MicrosoftAdpcmEncodeIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [TestCase("pcm16")]
        [Explicit("WIP")]
        public void Test1(string name)
        {
            var wavDecoder = Resolve<IWavDecoder>();
            var adpcmEncoder = Resolve<IRiffMicrosoftAdpcmSoundEncoder>();
            var writer = Resolve<IRiffStreamWriter>();
            
            var data = GetArchiveResource($"Wav.{name}.zip")
                .First()
                .Value;

            var sound = wavDecoder.Decode(new ReadOnlyMemoryStream(data));
            var encoded = adpcmEncoder.Encode(sound, 500);

            using var stream =
                File.OpenWrite(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "out-ms.wav"));
            writer.Write(stream, encoded);
            stream.Flush();
        }
    }
}