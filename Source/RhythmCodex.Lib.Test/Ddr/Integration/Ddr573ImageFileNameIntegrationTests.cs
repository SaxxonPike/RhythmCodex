using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Ddr.Processors;

namespace RhythmCodex.Ddr.Integration
{
    public class Ddr573ImageFileNameIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [TestCase(0x6EF410A0, "data/mdb/aaaa/all.csq")]
        [TestCase(0x7CF5A389, "data/mdb/aaaa/aaaa_bk.cmt")]
        public void Test_Hash(int hash, string name)
        {
            // Arrange.
            var subject = Resolve<Ddr573ImageFileNameHasher>();

            // Act.
            var observed = $"{subject.Calculate(name):X8}";

            // Assert.
            observed.Should().Be($"{hash:X8}");
        }
    }
}