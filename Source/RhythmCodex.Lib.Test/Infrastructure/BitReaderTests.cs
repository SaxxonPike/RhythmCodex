using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace RhythmCodex.Infrastructure;

public class BitReaderTests : BaseUnitTestFixture
{
    [Test]
    public void Test1()
    {
        var reader = new BitReader(new MemoryStream([
            0x00, 0xFF, 0x55, 0xAA, 0xC4
        ]));

        reader.Read(4).Should().Be(0x0);
        reader.Read(4).Should().Be(0x0);
        reader.Read(4).Should().Be(0xF);
        reader.Read(4).Should().Be(0xF);
        reader.Read(4).Should().Be(0x5);
        reader.Read(4).Should().Be(0x5);
        reader.Read(4).Should().Be(0xA);
        reader.Read(4).Should().Be(0xA);
        reader.Read(3).Should().Be(0x6);
        reader.Read(3).Should().Be(0x1);
        reader.Read(2).Should().Be(0x0);
    }
}