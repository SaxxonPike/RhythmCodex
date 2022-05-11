using System.IO;
using System.IO.Compression;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Infrastructure;
using RhythmCodex.Twinkle.Converters;
using RhythmCodex.Twinkle.Model;
using RhythmCodex.Twinkle.Streamers;

namespace RhythmCodex.OneShots;

[TestFixture]
public class TwinkleOneShots : BaseIntegrationFixture
{
    [Test]
    [Explicit("This is a tool, not a test.")]
    [TestCase(@"Z:\User Data\Bemani\Beatmania Non-PC\iidxsubstream.zip")]
    public void ExtractBms(string path)
    {
        var streamer = Resolve<ITwinkleBeatmaniaStreamReader>();
        var decoder = Resolve<ITwinkleBeatmaniaDecoder>();

        using var stream = File.OpenRead(path);
        using var zipStream = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = zipStream.Entries.Single();
        using var entryStream = entry.Open();

        var options = new TwinkleDecodeOptions();

        var index = 0;
        foreach (var chunk in streamer.Read(entryStream, entry.Length, true))
        {
            TestContext.Out.WriteLine($"Working on chunk {index}");
            var archive = decoder.Decode(chunk, options);
            if (archive != null)
            {
                var title = $"{Alphabet.EncodeNumeric(index, 4)}";
                var basePath = Path.Combine("2dx1ss", title);
                this.WriteSet(archive.Charts, archive.Samples, basePath, title);
            }

            index++;
        }
    }
}