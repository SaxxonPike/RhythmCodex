using FluentAssertions;
using NUnit.Framework;

namespace RhythmCodex.Djmain.Converters
{
    [TestFixture]
    public class DpcmAudioDecoderTests : BaseUnitTestFixture<AudioDecoder>
    {
        [Test]
        public void Decode_DecodesData()
        {
            // Arrange.
            var data = new byte[] {0x12};
            var expected = new[] {2/128f, 3/128f};

            // Act.
            var result = Subject.DecodeDpcm(data);

            // Assert.
            result.ShouldAllBeEquivalentTo(expected);
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
            result.ShouldAllBeEquivalentTo(expected);
        }
    }
}
