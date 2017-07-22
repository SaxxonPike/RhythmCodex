using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [TestFixture]
    public class TriggerDecoderTests : BaseTestFixture<TriggerDecoder>
    {
        [Test]
        public void Convert_DecodesTriggers()
        {
            // Arrange.
            var data = new byte[]
            {
                0x03, 0x00, 0x00, 0x00,

                0x12, 0x34,
                0x56, 0x78,
                0x90, 0x12
            };

            var expected = new[]
            {
                new Trigger {Type = 0x12, Parameter = 0x34},
                new Trigger {Type = 0x56, Parameter = 0x78},
                new Trigger {Type = 0x90, Parameter = 0x12}
            };

            // Act.
            var result = Subject.Convert(data);

            // Assert.
            result.ShouldAllBeEquivalentTo(expected);
        }
    }
}
