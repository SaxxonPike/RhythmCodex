using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Plugin.NVorbis;
using Shouldly;

namespace RhythmCodex.Sounds.Ogg;

[TestFixture]
public class OggDecoderTest : BaseIntegrationFixture<OggDecoder>
{
    [Test]
    [TestCase("Ogg.example.ogg.zip")]
    public void Decode_ShouldDecodeOgg(string fileName)
    {
        var data = GetArchiveResource(fileName).Single();
        using var stream = new MemoryStream(data.Value);
        var output = Subject.Decode(stream);
        output.ShouldNotBeNull();
        output.Samples.Count.ShouldBe(2);
    }
}