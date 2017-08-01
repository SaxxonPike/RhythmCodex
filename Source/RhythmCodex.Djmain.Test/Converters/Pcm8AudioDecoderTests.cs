using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace RhythmCodex.Djmain.Converters
{
    [TestFixture]
    public class Pcm8AudioDecoderTests : BaseUnitTestFixture<Pcm8AudioDecoder>
    {
        [Test]
        public void Decode_DecodesData()
        {
            // Arrange.
            var data = new byte[] {0x12, 0x34, 0x56, 0x78};
            var expected = data.Select(v => ((v ^ 0x80) - 0x80) / 128f);
            
            // Act.
            var result = Subject.Decode(data);
            
            // Assert.
            result.ShouldAllBeEquivalentTo(expected);
        }
    }
}
