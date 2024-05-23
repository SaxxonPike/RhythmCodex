using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

public class TriggerChunkEncoderTests : BaseUnitTestFixture<TriggerChunkEncoder, ITriggerChunkEncoder>
{
    [Test]
    public void Convert_EncodesTriggers()
    {
        // Arrange.
        var triggers = new[]
        {
            new Trigger {Id = 0x3412, MetricOffset = 0x30201000},
            new Trigger {Id = 0x7856, MetricOffset = 0x40302010},
            new Trigger {Id = 0x1290, MetricOffset = 0x50403020}
        };

        var expected = new byte[]
        {
            0x03, 0x00, 0x00, 0x00,

            0x00, 0x10, 0x20, 0x30,
            0x10, 0x20, 0x30, 0x40,
            0x20, 0x30, 0x40, 0x50,

            0x12, 0x34,
            0x56, 0x78,
            0x90, 0x12
        };

        // Act.
        var result = Subject.Convert(triggers);

        // Assert.
        result.Should().BeEquivalentTo(expected);
    }
}