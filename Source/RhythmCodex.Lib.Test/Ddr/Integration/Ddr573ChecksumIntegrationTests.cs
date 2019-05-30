using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Ddr.Converters;
using RhythmCodex.Ddr.Processors;
using RhythmCodex.Digital573.Converters;

namespace RhythmCodex.Ddr.Integration
{
    public class Ddr573ChecksumIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        public void Checksum_Blank()
        {
            // Arrange.
            var data = new byte[0];
            var subject = Resolve<Ddr573ChecksumCalculator>();

            // Act.
            var observed = $"{subject.CalculateChecksum(data):X8}";

            // Assert.
            observed.Should().Be("A8E06D56");
        }

        [Test]
        public void Checksum_30b()
        {
            // Arrange.
            var data = new byte[]
            {
                0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };
            var subject = Resolve<Ddr573ChecksumCalculator>();
            
            // Act.
            var observed = $"{subject.CalculateChecksum(data):X8}";
            
            // Assert.
            observed.Should().Be("13ACFCBC");
        }
    }
}