using System.Linq;
using Shouldly;
using NUnit.Framework;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Mappers;

[TestFixture]
public class Ddr2Player4PanelMapperTests : BaseUnitTestFixture<Ddr2Player4PanelMapper, IPanelMapper>
{
    [Test]
    public void Map_Int_CorrectlyMaps()
    {
        // Arrange.
        var expected = new[]
        {
            new PanelMapping {Player = 0, Panel = 0},
            new PanelMapping {Player = 0, Panel = 1},
            new PanelMapping {Player = 0, Panel = 2},
            new PanelMapping {Player = 0, Panel = 3},
            new PanelMapping {Player = 1, Panel = 0},
            new PanelMapping {Player = 1, Panel = 1},
            new PanelMapping {Player = 1, Panel = 2},
            new PanelMapping {Player = 1, Panel = 3},
            null
        };

        // Act.
        var result = Enumerable.Range(0, expected.Length).Select(Subject.Map);

        // Assert.
        result.ShouldBe(expected);
    }

    [Test]
    public void Map_PanelMapping_CorrectlyMaps()
    {
        // Arrange.
        var expected = new int?[] {0, 1, 2, 3, 4, 5, 6, 7, null, null, null};
        var data = new[]
        {
            new PanelMapping {Player = 0, Panel = 0},
            new PanelMapping {Player = 0, Panel = 1},
            new PanelMapping {Player = 0, Panel = 2},
            new PanelMapping {Player = 0, Panel = 3},
            new PanelMapping {Player = 1, Panel = 0},
            new PanelMapping {Player = 1, Panel = 1},
            new PanelMapping {Player = 1, Panel = 2},
            new PanelMapping {Player = 1, Panel = 3},
            new PanelMapping {Player = 0, Panel = 4},
            new PanelMapping {Player = 1, Panel = 4},
            new PanelMapping {Player = 2, Panel = 0}
        };

        // Act.
        var result = data.Select(Subject.Map);

        // Assert.
        result.ShouldBe(expected);
    }
}