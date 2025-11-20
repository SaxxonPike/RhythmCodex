using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace RhythmCodex.Archs.Djmain.Converters;

[TestFixture]
public class AudioDecoderTests : BaseUnitTestFixture<DjmainAudioDecoder, IDjmainAudioDecoder>
{
    [Test]
    public void Decode_DecodesData()
    {
        // Arrange.
        var data = new byte[] {0x12};
        var expected = new[] {2 / 128f, 3 / 128f};

        // Act.
        var result = Subject.DecodeDpcm(data);

        // Assert.
        result.ShouldBeEquivalentTo(expected);
    }

    [Test]
    [TestCase(0x0, 0 / 128f)]
    [TestCase(0x1, 1 / 128f)]
    [TestCase(0x2, 2 / 128f)]
    [TestCase(0x3, 4 / 128f)]
    [TestCase(0x4, 8 / 128f)]
    [TestCase(0x5, 16 / 128f)]
    [TestCase(0x6, 32 / 128f)]
    [TestCase(0x7, 64 / 128f)]
    [TestCase(0x8, 0 / 128f)]
    [TestCase(0x9, -64 / 128f)]
    [TestCase(0xA, -32 / 128f)]
    [TestCase(0xB, -16 / 128f)]
    [TestCase(0xC, -8 / 128f)]
    [TestCase(0xD, -4 / 128f)]
    [TestCase(0xE, -2 / 128f)]
    [TestCase(0xF, -1 / 128f)]
    public void Decode_UsesProperValues(int inputValue, float expectedValue)
    {
        // Arrange.
        var data = new[] {unchecked((byte) inputValue)};
        var expected = new[] {expectedValue, expectedValue};

        // Act.
        var result = Subject.DecodeDpcm(data);

        // Assert.
        result.ShouldBeEquivalentTo(expected);
    }

    [Test]
    public void DecodePcm16_DecodesData()
    {
        // Arrange.
        var data = new byte[] { 0x12, 0x34, 0x56, 0x78, 0xFF, 0xFF };
        var expected = new[] { 0x3412 / 32768f, 0x7856 / 32768f, -1 / 32768f };

        // Act.
        var result = Subject.DecodePcm16(data);

        // Assert.
        result.ShouldBeEquivalentTo(expected);
    }

    [Test]
    public void DecodePcm16_DecodesPartialData()
    {
        // Arrange.
        var data = new byte[] {0x12, 0x34, 0x56};
        var expected = new[] {0x3412 / 32768f, 0x0056 / 32768f};

        // Act.
        var result = Subject.DecodePcm16(data);

        // Assert.
        result.ShouldBeEquivalentTo(expected);
    }

    [Test]
    public void DecodePcm8_DecodesData()
    {
        // Arrange.
        var data = new byte[256];
        for (var i = 0; i < 256; i++)
            data[i] = unchecked((byte)i);

        var expected = data
            .Select(v => ((v << 24) >> 24) / 128f)
            .ToArray();

        // Act.
        var result = Subject.DecodePcm8(data);

        // Assert.
        result.ShouldBeEquivalentTo(expected);
    }
}