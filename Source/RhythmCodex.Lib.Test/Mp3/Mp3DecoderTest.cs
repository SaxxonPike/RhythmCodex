using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Plugin.MP3Sharp;

namespace RhythmCodex.Mp3;

[TestFixture]
public class Mp3DecoderTest : BaseUnitTestFixture<Mp3Decoder>
{
    [Test]
    [TestCase("Mp3.example.mp3.zip")]
    public void Decode_ShouldDecodeMp3(string fileName)
    {
        var data = GetArchiveResource(fileName).Single();
        using var stream = new MemoryStream(data.Value);
        var output = Subject.Decode(stream);
        output.Should().NotBeNull();
        output.Samples.Should().HaveCount(2);
    }
}