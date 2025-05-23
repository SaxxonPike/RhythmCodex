using Shouldly;
using NUnit.Framework;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[TestFixture]
public class StepChunkEncoderTests : BaseUnitTestFixture<StepChunkEncoder, IStepChunkEncoder>
{
    [Test]
    public void Convert_EncodesFreezeSteps()
    {
        // Arrange.
        var steps = new[]
        {
            new Step {MetricOffset = 0x123456, Panels = 0x00, ExtraPanels = 0x02, ExtraPanelInfo = 0x01},
            new Step {MetricOffset = 0x234567, Panels = 0x00, ExtraPanels = 0x20, ExtraPanelInfo = 0x02},
            new Step {MetricOffset = 0x345678, Panels = 0x00, ExtraPanels = 0x80, ExtraPanelInfo = 0x03}
        };

        var expected = new byte[]
        {
            0x03, 0x00, 0x00, 0x00,

            0x56, 0x34, 0x12, 0x00,
            0x67, 0x45, 0x23, 0x00,
            0x78, 0x56, 0x34, 0x00,

            0x00,
            0x00,
            0x00,

            0x00,

            0x02, 0x01,
            0x20, 0x02,
            0x80, 0x03
        };

        // Act.
        var result = Subject.Convert(steps);

        // Assert.
        result.ToArray().ShouldBe(expected);
    }

    [Test]
    public void ConvertEncodesRegularSteps()
    {
        // Arrange.
        var steps = new[]
        {
            new Step {MetricOffset = 0x123456, Panels = 0x01},
            new Step {MetricOffset = 0x234567, Panels = 0x10},
            new Step {MetricOffset = 0x345678, Panels = 0x40}
        };

        var expected = new byte[]
        {
            0x03, 0x00, 0x00, 0x00,

            0x56, 0x34, 0x12, 0x00,
            0x67, 0x45, 0x23, 0x00,
            0x78, 0x56, 0x34, 0x00,

            0x01,
            0x10,
            0x40,

            0x00
        };

        // Act.
        var result = Subject.Convert(steps);

        // Assert.
        result.ToArray().ShouldBe(expected);
    }
}