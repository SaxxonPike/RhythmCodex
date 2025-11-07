using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Archs.Djmain.Model;
using Shouldly;

namespace RhythmCodex.Archs.Djmain.Streamers;

[TestFixture]
public class DjmainSampleDefinitionStreamReaderTests : BaseUnitTestFixture<DjmainSampleInfoStreamReader,
    IDjmainSampleInfoStreamReader>
{
    [Test]
    public void Read_ShouldReadAllDefinitions()
    {
        // Arrange.
        Mock<IDjmainConfiguration>().Setup(x => x.MaxSampleDefinitions).Returns(3);

        var data = new byte[]
        {
            0x12, 0x34, 0x56, 0x78, 0x90, 0x12, 0x34, 0x56, 0x78, 0x90, 0x12,
            0x34, 0x56, 0x78, 0x90, 0x12, 0x34, 0x56, 0x78, 0x90, 0x12, 0x34
        };

        // Act.
        using var mem = new MemoryStream(data);
        var output = Subject.Read(mem).ToArray();

        // Assert.
        output.Length.ShouldBe(2);
        output[0].Key.ShouldBe(0);
        output[0].Value.ShouldBeEquivalentTo(new DjmainSampleInfo
        {
            Channel = 0x12,
            Frequency = 0x5634,
            ReverbVolume = 0x78,
            Volume = 0x90,
            Panning = 0x12,
            Offset = 0x785634,
            SampleType = 0x90,
            Flags = 0x12
        });
        output[1].Key.ShouldBe(1);
        output[1].Value.ShouldBeEquivalentTo(new DjmainSampleInfo
        {
            Channel = 0x34,
            Frequency = 0x7856,
            ReverbVolume = 0x90,
            Volume = 0x12,
            Panning = 0x34,
            Offset = 0x907856,
            SampleType = 0x12,
            Flags = 0x34
        });
    }
}