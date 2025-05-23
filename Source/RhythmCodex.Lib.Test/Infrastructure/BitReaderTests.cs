using System.IO;
using Shouldly;
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

        reader.Read(4).ShouldBe(0x0);
        reader.Read(4).ShouldBe(0x0);
        reader.Read(4).ShouldBe(0xF);
        reader.Read(4).ShouldBe(0xF);
        reader.Read(4).ShouldBe(0x5);
        reader.Read(4).ShouldBe(0x5);
        reader.Read(4).ShouldBe(0xA);
        reader.Read(4).ShouldBe(0xA);
        reader.Read(3).ShouldBe(0x6);
        reader.Read(3).ShouldBe(0x1);
        reader.Read(2).ShouldBe(0x0);
    }
}