using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Plugin.MP3Sharp;
using Shouldly;

namespace RhythmCodex.Sounds.Mp3;

[TestFixture]
public class Mp3DecoderTest : BaseIntegrationFixture<Mp3Decoder>
{
    [Test]
    [TestCase("Mp3.example.mp3.zip")]
    public void Decode_ShouldDecodeMp3(string fileName)
    {
        var data = GetArchiveResource(fileName).Single();
        using var stream = new MemoryStream(data.Value);
        var output = Subject.Decode(stream);
        output.ShouldNotBeNull();
        output.Samples.Count.ShouldBe(2);
    }
}