using Shouldly;
using NUnit.Framework;
using RhythmCodex.Ddr.Processors;
using RhythmCodex.Ddr.S573.Processors;

namespace RhythmCodex.Ddr.Integration;

public class Ddr573ImageFileNameIntegrationTests : BaseIntegrationFixture
{
    [Test]
    [TestCase(0x6EF410A0, "data/mdb/aaaa/all.csq")]
    [TestCase(0x7CF5A389, "data/mdb/aaaa/aaaa_bk.cmt")]
    [TestCase(0xCC8A6B44, "data/mdb/mdb.bin")]
    public void Test_Hash(long hash, string name)
    {
        // Arrange.
        var subject = Resolve<Ddr573ImageFileNameHasher>();

        // Act.
        var observed = $"{subject.Calculate(name):X8}";

        // Assert.
        observed.ShouldBe($"{hash:X8}");
    }
}