using System;
using System.Buffers.Text;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Ddr.Converters;
using RhythmCodex.Ddr.Streamers;

namespace RhythmCodex.Ddr.Integration
{
    [TestFixture]
    public class Ddr573AudioIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [Explicit("wip")]
        public void DecryptNewTest()
        {
            // 3626_20f7_6b
            var inputArchive = GetArchiveResource($"Ddr.mp3.zip");
            var data = inputArchive
                .First(name => name.Key.EndsWith(".dat", StringComparison.OrdinalIgnoreCase))
                .Value;
            var expected = inputArchive
                .First(name => name.Key.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                .Value;
            var decrypter = Resolve<IDdr573AudioDecrypter>();
            var observed = decrypter.DecryptNew(data, 0x3626, 0x20F7, 0x6B);
            observed.Should().Equal(expected);
        }

        [Test]
        [Explicit("wip")]
        [TestCase("sbm1", 0x1C67)]
        [TestCase("sbm2", 0x6546)]
        public void DecryptOldTest(string archiveName, int key)
        {
            var inputArchive = GetArchiveResource($"Ddr.{archiveName}.zip");
            var data = inputArchive
                .First(name => name.Key.EndsWith(".dat", StringComparison.OrdinalIgnoreCase))
                .Value;
            var expected = inputArchive
                .First(name => name.Key.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                .Value;
            var decrypter = Resolve<IDdr573AudioDecrypter>();
            var observed = decrypter.DecryptOld(data, key);
            observed.Should().Equal(expected);
        }
    }
}