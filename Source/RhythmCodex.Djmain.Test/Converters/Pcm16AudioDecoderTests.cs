using FluentAssertions;
using NUnit.Framework;

namespace RhythmCodex.Djmain.Converters
{
    [TestFixture]
    public class Pcm16AudioDecoderTests : BaseUnitTestFixture<Pcm16AudioDecoder>
    {
        [Test]
        public void Decode_DecodesData()
        {
            // Arrange.
            var data = new byte[] {0x12, 0x34, 0x56, 0x78};
            var expected = new[] {0x3412 / 32768f, 0x7856 / 32768f};

            // Act.
            var result = Subject.Decode(data);

            // Assert.
            result.ShouldAllBeEquivalentTo(expected);
        }

        [Test]
        public void Decode_DecodesPartialData()
        {
            // Arrange.
            var data = new byte[] { 0x12, 0x34, 0x56 };
            var expected = new[] { 0x3412 / 32768f, 0x0056 / 32768f };

            // Act.
            var result = Subject.Decode(data);

            // Assert.
            result.ShouldAllBeEquivalentTo(expected);
        }

    }
}
