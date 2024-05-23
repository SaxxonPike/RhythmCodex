using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace RhythmCodex.Streamers;

public class ByteSwappedReadStreamTests : BaseUnitTestFixture
{
    [Test]
    public void Read_ReadsCorrectBytes()
    {
        // Arrange.
        var data = CreateMany<byte>(4).ToArray();
        var stream = new MemoryStream(data);

        // Act.
        var subject = new ByteSwappedReadStream(stream);
        var result = new byte[3];
        subject.Read(result, 0, result.Length);

        // Assert.
        result.Should().BeEquivalentTo(new[] {data[1], data[0], data[3]});
    }

    [Test]
    public void ReadByte_ReadsCorrectByte()
    {
        // Arrange.
        var data = CreateMany<byte>(4).ToArray();
        var stream = new MemoryStream(data);

        // Act.
        var subject = new ByteSwappedReadStream(stream);

        // Assert.
        var firstResult = subject.ReadByte();
        firstResult.Should().Be(data[1]);
        var secondResult = subject.ReadByte();
        secondResult.Should().Be(data[0]);
        var thirdResult = subject.ReadByte();
        thirdResult.Should().Be(data[3]);
    }
}