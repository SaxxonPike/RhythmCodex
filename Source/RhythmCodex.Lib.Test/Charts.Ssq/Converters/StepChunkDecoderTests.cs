using NUnit.Framework;
using RhythmCodex.Charts.Ssq.Model;
using Shouldly;

namespace RhythmCodex.Charts.Ssq.Converters;

[TestFixture]
public class StepChunkDecoderTests : BaseUnitTestFixture<StepChunkDecoder, IStepChunkDecoder>
{
    [Test]
    public void Convert_DecodesFreezeSteps()
    {
        // Arrange.
        var data = new byte[]
        {
            0x03, 0x00, 0x00, 0x00, // length in records

            0x56, 0x34, 0x12, 0x00, // offset 1
            0x67, 0x45, 0x23, 0x00, // offset 2
            0x78, 0x56, 0x34, 0x00, // offset 3

            0x00, // freeze 1
            0x00, // freeze 2
            0x00, // freeze 3

            0x00, // padding due to odd number of panel data

            0x02, 0x01, // freeze info 1
            0x20, 0x02, // freeze info 2
            0x80, 0x03 // freeze info 3
        };

        var expected = new[]
        {
            new Step {MetricOffset = 0x123456, Panels = 0x00, ExtraPanels = 0x02, ExtraPanelInfo = 0x01},
            new Step {MetricOffset = 0x234567, Panels = 0x00, ExtraPanels = 0x20, ExtraPanelInfo = 0x02},
            new Step {MetricOffset = 0x345678, Panels = 0x00, ExtraPanels = 0x80, ExtraPanelInfo = 0x03}
        };

        // Act.
        var result = Subject.Convert(data);

        // Assert.
        result.ShouldBe(expected);
    }

    [Test]
    public void Convert_DecodesRegularSteps()
    {
        // Arrange.
        var data = new byte[]
        {
            0x03, 0x00, 0x00, 0x00, // length in records

            0x56, 0x34, 0x12, 0x00, // offset 1
            0x67, 0x45, 0x23, 0x00, // offset 2
            0x78, 0x56, 0x34, 0x00, // offset 3

            0x01, // step 1
            0x10, // step 2
            0x40, // step 3
            0x00 // padding
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
        result.ShouldBe(expected);
    }
}