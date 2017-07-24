using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [TestFixture]
    public class StepDecoderTests : BaseUnitTestFixture<StepChunkDecoder>
    {
        [Test]
        public void Convert_DecodesRegularSteps()
        {
            // Arrange.
            var data = new byte[]
            {
                0x03, 0x00, 0x00, 0x00,

                0x56, 0x34, 0x12, 0x00,
                0x67, 0x45, 0x23, 0x00,
                0x78, 0x56, 0x34, 0x00,

                0x01,
                0x10,
                0x40
            };

            var expected = new[]
            {
                new Step {MetricOffset = 0x123456, Panels = 0x01},
                new Step {MetricOffset = 0x234567, Panels = 0x10},
                new Step {MetricOffset = 0x345678, Panels = 0x40}
            };

            // Act.
            var result = Subject.Convert(data);

            // Assert.
            result.ShouldAllBeEquivalentTo(expected);
        }

        [Test]
        public void Convert_DecodesFreezeSteps()
        {
            // Arrange.
            var data = new byte[]
            {
                0x03, 0x00, 0x00, 0x00,

                0x56, 0x34, 0x12, 0x00,
                0x67, 0x45, 0x23, 0x00,
                0x78, 0x56, 0x34, 0x00,

                0x00,
                0x00,
                0x00,

                0x02,
                0x20,
                0x80
            };

            var expected = new[]
            {
                new Step {MetricOffset = 0x123456, Panels = 0x00, ExtraPanels = 0x02 },
                new Step {MetricOffset = 0x234567, Panels = 0x00, ExtraPanels = 0x20 },
                new Step {MetricOffset = 0x345678, Panels = 0x00, ExtraPanels = 0x80 }
            };

            // Act.
            var result = Subject.Convert(data);

            // Assert.
            result.ShouldAllBeEquivalentTo(expected);
        }
    }
}
