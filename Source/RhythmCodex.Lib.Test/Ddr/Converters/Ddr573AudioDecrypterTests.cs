using FluentAssertions;
using NUnit.Framework;

namespace RhythmCodex.Ddr.Converters
{
    public class Ddr573AudioDecrypterTests : BaseUnitTestFixture<Ddr573AudioDecrypter>
    {
        [Test]
        [TestCase("M5BZYH13", "LDYN")]
        [TestCase("M5BZYH13.DAT", "LDYN")]
        [TestCase("MABC1RWH", "WILD")]
        public void Test1(string input, string expected)
        {
            var observed = Subject.ExtractName(input);
            observed.Should().Be(expected, "name must be extracted correctly");
        }
    }
}