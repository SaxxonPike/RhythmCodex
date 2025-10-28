using System.IO;
using System.Linq;
using Shouldly;
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
        subject.Read(result, 0, result.Length).ShouldBe(3);

        // Assert.
        result.ShouldBeEquivalentTo(new[] {data[1], data[0], data[3]});
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
        firstResult.ShouldBe(data[1]);
        var secondResult = subject.ReadByte();
        secondResult.ShouldBe(data[0]);
        var thirdResult = subject.ReadByte();
        thirdResult.ShouldBe(data[3]);
    }
}