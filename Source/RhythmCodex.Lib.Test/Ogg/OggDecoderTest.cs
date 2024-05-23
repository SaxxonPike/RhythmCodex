using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Plugin.NVorbis;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Ogg;

[TestFixture]
public class OggDecoderTest : BaseUnitTestFixture<OggDecoder>
{
    [Test]
    [TestCase("Ogg.example.ogg.zip")]
    public void Decode_ShouldDecodeOgg(string fileName)
    {
        var data = GetArchiveResource(fileName).Single();
        using var stream = new MemoryStream(data.Value);
        var output = Subject.Decode(stream);
        output.Should().NotBeNull();
        output.Samples.Should().HaveCount(2);
    }
}