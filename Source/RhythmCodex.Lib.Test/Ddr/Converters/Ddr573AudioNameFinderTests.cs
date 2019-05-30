using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Ddr.Processors;
using RhythmCodex.Digital573.Converters;

namespace RhythmCodex.Ddr.Converters
{
    public class Ddr573AudioNameFinderTests : BaseUnitTestFixture<Ddr573AudioNameFinder>
    {
        [Test]
        [TestCase("M5BZYH13", "LDYN")]
        [TestCase("M5BZYH13.DAT", "LDYN")]
        [TestCase("MABD1RWH", "WILD")]
        public void GetName_ShouldExtractNameFromFileName(string input, string expected)
        {
            var observed = Subject.GetName(input);
            observed.Should().Be(expected, "name must be extracted correctly");
        }
    }
}