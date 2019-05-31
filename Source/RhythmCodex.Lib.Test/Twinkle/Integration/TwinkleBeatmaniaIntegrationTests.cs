using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Meta.Models;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Twinkle.Converters;
using RhythmCodex.Twinkle.Streamers;

namespace RhythmCodex.Twinkle.Integration
{
    public class TwinkleBeatmaniaIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [Explicit("wip")]
        public void Test1()
        {
            var streamer = Resolve<ITwinkleBeatmaniaStreamReader>();

            using (var stream = File.OpenRead(@"Z:\Bemani\Beatmania Non-PC\iidx8th.zip"))
            using (var zipStream = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                var entry = zipStream.Entries.Single();
                using (var entryStream = entry.Open())
                {
                    var chunks = streamer.Read(entryStream, stream.Length);
                    var chunk = chunks.First();
                    File.WriteAllBytes(@"c:\users\saxxon\desktop\twinkle.bin", chunk.Data);
                }
            }
        }

        [Test]
        [Explicit("wip")]
        public void Test2()
        {
            // Arrange.
            var data = GetArchiveResource("Twinkle.8th.zip")
                .First()
                .Value;
            var streamer = Resolve<ITwinkleBeatmaniaStreamReader>();
            var decoder = Resolve<ITwinkleBeatmaniaDecoder>();

            // Act.
            var chunk = streamer.Read(new MemoryStream(data), data.Length, false).First();
            var archive = decoder.Decode(chunk);

            // Assert.
            foreach (var sound in archive.Samples)
            {
                this.WriteSound(sound, Path.Combine("bmiidx", $"{(int)sound[NumericData.Id]:D3}.wav"));
            }
        }
    }
}