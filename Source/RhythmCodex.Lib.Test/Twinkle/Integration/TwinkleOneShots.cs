using System.IO;
using System.IO.Compression;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Bms.Converters;
using RhythmCodex.Bms.Streamers;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;
using RhythmCodex.Twinkle.Converters;
using RhythmCodex.Twinkle.Streamers;

namespace RhythmCodex.Twinkle.Integration;

[TestFixture]
public class TwinkleOneShots : BaseIntegrationFixture
{
    [Test]
    [Explicit("This is a tool, not a test.")]
    [TestCase(@"Z:\User Data\Bemani\Beatmania Non-PC\iidx5th.zip")]
    public void ExtractBms(string path)
    {
        // Arrange.
        var streamer = Resolve<ITwinkleBeatmaniaStreamReader>();
        var decoder = Resolve<ITwinkleBeatmaniaDecoder>();
        var bmsEncoder = Resolve<IBmsEncoder>();
        var bmsWriter = Resolve<IBmsStreamWriter>();

        // Act.
        using var stream = File.OpenRead(@"Z:\User Data\Bemani\Beatmania Non-PC\iidx5th.zip");
        using var zipStream = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = zipStream.Entries.Single();
        using var entryStream = entry.Open();

        var index = 0;
        foreach (var chunk in streamer.Read(entryStream, entry.Length, true))
        {
            var archive = decoder.Decode(chunk);
            var basePath = Path.Combine("bmiidx", $"{Alphabet.EncodeNumeric(index, 4)}");

            foreach (var sound in archive.Samples.Where(s => s.Samples.Any()))
            {
                this.WriteSound(sound, Path.Combine(basePath, $"{Alphabet.EncodeAlphanumeric((int)sound[NumericData.Id], 4)}.wav"));
            }

            foreach (var chart in archive.Charts)
            {
                chart.PopulateMetricOffsets();
                using var outStream = this.OpenWrite(Path.Combine(basePath, $"{(int)chart[NumericData.ByteOffset]}.bms"));
                bmsWriter.Write(outStream, bmsEncoder.Encode(chart));
                outStream.Flush();
            }
                
            index++;
        }
    }
}