using System;
using System.IO;
using NUnit.Framework;
using RhythmCodex.Chd.Streamers;

namespace RhythmCodex.Chd.Integration
{
    public class ChdIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [Explicit]
        public void test1()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "753jaa11.chd");
            var factory = Resolve<IChdStreamFactory>();
            using var stream = File.OpenRead(path);
            var chd = factory.Create(stream);
            chd.Position = 0;
            var reader = new BinaryReader(chd);
            var output = reader.ReadBytes(0x2000000);
            File.WriteAllBytes(@"c:\users\saxxon\desktop\chd.bin", output);
        }
    }
}