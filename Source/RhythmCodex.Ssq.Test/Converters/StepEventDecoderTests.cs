using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Charting;
using RhythmCodex.Extensions;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [TestFixture]
    public class StepEventDecoderTests : BaseUnitTestFixture<StepEventDecoder>
    {
        [Test]
        public void Decode_ConvertsStepsCorrectly()
        {
            Mock<IPanelMapper>(mock =>
            {
                mock.Setup(x => x.Map(0)).Returns<int>(i => new PanelMapping { Panel = 11, Player = 1 });
                mock.Setup(x => x.Map(1)).Returns<int>(i => new PanelMapping { Panel = 22, Player = 2 });
                mock.Setup(x => x.Map(2)).Returns<int>(i => new PanelMapping { Panel = 33, Player = 3 });
                mock.Setup(x => x.Map(3)).Returns<int>(i => new PanelMapping { Panel = 44, Player = 4 });
            });

            var steps = new[]
            {
                new Step {Panels = 0x01, MetricOffset = 1234},
                new Step {Panels = 0x12, MetricOffset = 2345},
                new Step {Panels = 0x44, MetricOffset = 3456},
                new Step {Panels = 0x09, MetricOffset = 4567}
            };

            var expected = new[]
            {
                new Event {[NumericData.MetricOffset] = 1234, [NumericData.Panel] = 11, [NumericData.SourcePanel] = 0, [NumericData.Player] = 1 },
                new Event {[NumericData.MetricOffset] = 2345, [NumericData.Panel] = 22, [NumericData.SourcePanel] = 1, [NumericData.Player] = 2 },
                new Event {[NumericData.MetricOffset] = 2345, [NumericData.SourcePanel] = 4},
                new Event {[NumericData.MetricOffset] = 3456, [NumericData.Panel] = 33, [NumericData.SourcePanel] = 2, [NumericData.Player] = 3 },
                new Event {[NumericData.MetricOffset] = 3456, [NumericData.SourcePanel] = 6},
                new Event {[NumericData.MetricOffset] = 4567, [NumericData.Panel] = 11, [NumericData.SourcePanel] = 0, [NumericData.Player] = 1 },
                new Event {[NumericData.MetricOffset] = 4567, [NumericData.Panel] = 44, [NumericData.SourcePanel] = 3, [NumericData.Player] = 4 }
            };

            // Act.
            var result = Subject.Decode(steps).AsList();
            
            // Assert.
            result.Should().HaveCount(expected.Length);
            var resultMatches = Enumerable.Range(0, expected.Length)
                .Select(i => ((Event) result[i]).MetadataEquals(expected[i]));
            resultMatches.ShouldAllBeEquivalentTo(Enumerable.Repeat(true, expected.Length));
        }
    }
}
