using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public class TriggerEncoderTests : BaseTestFixture<TriggerEncoder>
    {
        [Test]
        public void Convert_EncodesTriggers()
        {
            // Arrange.
            var triggers = new[]
            {
                new Trigger {Type = 0x12, Parameter = 0x34},
                new Trigger {Type = 0x56, Parameter = 0x78},
                new Trigger {Type = 0x90, Parameter = 0x12}
            };

            var expected = new byte[]
            {
                0x03, 0x00, 0x00, 0x00,

                0x12, 0x34,
                0x56, 0x78,
                0x90, 0x12
            };

            // Act.
            var result = Subject.Convert(triggers);

            // Assert.
            result.ShouldAllBeEquivalentTo(expected);
        }
    }
}
