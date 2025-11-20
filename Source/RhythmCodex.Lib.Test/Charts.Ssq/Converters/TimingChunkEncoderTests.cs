using NUnit.Framework;
using RhythmCodex.Charts.Ssq.Model;
using Shouldly;

namespace RhythmCodex.Charts.Ssq.Converters;

[TestFixture]
public class TimingChunkEncoderTests : BaseUnitTestFixture<TimingChunkEncoder, ITimingChunkEncoder>
{
    [Test]
    public void Convert_EncodesTimings()
    {
        // Arrange.
        var timings = new[]
        {
            new Timing {LinearOffset = 0x123456, MetricOffset = 0x345678},
            new Timing {LinearOffset = 0x234567, MetricOffset = 0x456789},
            new Timing {LinearOffset = 0x345678, MetricOffset = 0x567890}
        };

        var expected = new byte[]
        {
            0x03, 0x00, 0x00, 0x00,

            0x78, 0x56, 0x34, 0x00,
            0x89, 0x67, 0x45, 0x00,
            0x90, 0x78, 0x56, 0x00,

            0x56, 0x34, 0x12, 0x00,
            0x67, 0x45, 0x23, 0x00,
            0x78, 0x56, 0x34, 0x00
        };

        // Act.
        var result = Subject.Convert(timings);

        // Assert.
        result.ToArray().ShouldBeEquivalentTo(expected);
    }
}