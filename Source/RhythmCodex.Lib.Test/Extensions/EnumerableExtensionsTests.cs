using System.Linq;
using Shouldly;
using NUnit.Framework;

namespace RhythmCodex.Extensions;

[TestFixture]
public class EnumerableExtensionsTests : BaseTestFixture
{
    [Test]
    public void AsList_ConvertsNonListsToList()
    {
        // Arrange.
        var data = CreateMany<int>().Select(i => i).ToArray();

        // Act.
        var output = data.Select(d => d);

        // Assert.
        output.ShouldBe(data);
        output.ShouldNotBeSameAs(data);
    }

    [Test]
    public void AsList_DoesNotConvertLists()
    {
        // Arrange.
        var data = CreateMany<int>().ToArray();

        // Act.
        var output = data;

        // Assert.
        output.ShouldBe(data);
        output.ShouldBeSameAs(data);
    }
}