using Shouldly;
using NUnit.Framework;
using RhythmCodex.Ddr.Processors;

namespace RhythmCodex.Ddr.Converters;

public class Ddr573AudioNameFinderTests : BaseUnitTestFixture<Ddr573AudioNameFinder>
{
    [Test]
    [TestCase("M5BZYH13", "LDYN")]
    [TestCase("M5BZYH13.DAT", "LDYN")]
    [TestCase("MABD1RWH", "WILD")]
    [TestCase("EABD1RWH", "WILD")]
    public void GetName_ShouldExtractNameFromFileName(string input, string expected)
    {
        var observed = Subject.GetName(input);
        observed.ShouldBe(expected, "name must be extracted correctly");
    }
        
    [Test]
    [TestCase("M5BZYH13", "data/mp3/enc/ldyn/ldyn.mp3")]
    [TestCase("M5BZYH13.DAT", "data/mp3/enc/ldyn/ldyn.mp3")]
    [TestCase("MABD1RWH", "data/mp3/enc/wild/wild.mp3")]
    [TestCase("SABD1RWH", "data/mp3/enc/wild/wild-preview.mp3")]
    [TestCase("EABD1RWH", "data/mp3/enc/wild.mp3")]
    public void GetPath_ShouldExtractAndBuildPath(string input, string expected)
    {
        var observed = Subject.GetPath(input);
        observed.ShouldBe(expected, "name must be extracted correctly");
    }
}