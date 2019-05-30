using System.IO;
using System.IO.Compression;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Twinkle.Streamers;

namespace RhythmCodex.Twinkle.Integration
{
    public class TwinkleBeatmaniaIntegrationTests : BaseIntegrationFixture
    {
        [Test]
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
    }
}