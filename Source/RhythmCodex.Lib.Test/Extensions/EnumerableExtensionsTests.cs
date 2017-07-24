using System.Collections.Generic;
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
            var data = new HashSet<int> {1, 2, 3};
            
            // Act.
            var output = data.AsList();
            
            // Assert.
            output.ShouldAllBeEquivalentTo(data);
            output.Should().NotBeSameAs(data);
        }

        [Test]
        public void AsList_DoesNotConvertLists()
        {
            // Arrange.
            var data = new List<int> {1, 2, 3};

            // Act.
            var output = data.AsList();

            // Assert.
            output.ShouldAllBeEquivalentTo(data);
            output.Should().BeSameAs(data);
        }

    }
}
