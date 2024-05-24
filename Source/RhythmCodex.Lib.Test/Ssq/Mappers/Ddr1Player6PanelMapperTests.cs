using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Mappers;

[TestFixture]
public class Ddr1Player6PanelMapperTests : BaseUnitTestFixture<Ddr1Player6PanelMapper, IPanelMapper>
{
    [Test]
    public void Map_Int_CorrectlyMaps()
    {
        // Arrange.
        var expected = new[]
        {
            new PanelMapping {Player = 0, Panel = 0},
            new PanelMapping {Player = 0, Panel = 2},
            new PanelMapping {Player = 0, Panel = 3},
            new PanelMapping {Player = 0, Panel = 5},
            new PanelMapping {Player = 0, Panel = 1},
            null,
            new PanelMapping {Player = 0, Panel = 4},
            null
        };

        // Act.
        var result = Enumerable.Range(0, expected.Length).Select(Subject.Map);

        // Assert.
        result.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void Map_PanelMapping_CorrectlyMaps()
    {
        // Arrange.
        var expected = new int?[] {0, 1, 2, 3, 4, 6, null, null};
        var data = new[]
        {
            new PanelMapping {Player = 0, Panel = 0},
            new PanelMapping {Player = 0, Panel = 2},
            new PanelMapping {Player = 0, Panel = 3},
            new PanelMapping {Player = 0, Panel = 5},
            new PanelMapping {Player = 0, Panel = 1},
            new PanelMapping {Player = 0, Panel = 4},
            new PanelMapping {Player = 0, Panel = 6},
            new PanelMapping {Player = 1, Panel = 0}
        };

        // Act.
        var result = data.Select(Subject.Map);

        // Assert.
        result.Should().BeEquivalentTo(expected);
    }
}