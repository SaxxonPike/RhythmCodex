using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace RhythmCodex.Extensions
{
    [TestFixture]
    public class EnumerableExtensionsTests : BaseTestFixture
    {
        [Test]
        public void AsList_ConvertsNonListsToList()
        {
            // Arrange.
            var data = CreateMany<int>().Select(i => i).ToArray();
            
            // Act.
            var output = data.Select(d => d).AsList();
            
            // Assert.
            output.ShouldAllBeEquivalentTo(data);
            output.Should().NotBeSameAs(data);
        }

        [Test]
        public void AsList_DoesNotConvertLists()
        {
            // Arrange.
            var data = CreateMany<int>().ToArray();

            // Act.
            var output = data.AsList();

            // Assert.
            output.ShouldAllBeEquivalentTo(data);
            output.Should().BeSameAs(data);
        }

    }
}
