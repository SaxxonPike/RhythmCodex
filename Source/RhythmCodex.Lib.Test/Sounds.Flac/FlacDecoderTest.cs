using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Plugin.CSCore;
using Shouldly;

namespace RhythmCodex.Sounds.Flac;

[TestFixture]
public class FlacDecoderTest : BaseIntegrationFixture<FlacDecoder>
{
    [Test]
    [TestCase("Flac.example.flac.zip")]
    public void Decode_ShouldDecodeFlac(string fileName)
    {
        var data = GetArchiveResource(fileName).Single();
        using var stream = new MemoryStream(data.Value);
        var output = Subject.Decode(stream);
        output.ShouldNotBeNull();
        output.Samples.Count.ShouldBe(1);
    }
}