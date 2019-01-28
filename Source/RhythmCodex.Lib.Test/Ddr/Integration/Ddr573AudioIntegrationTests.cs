using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Ddr.Streamers;

namespace RhythmCodex.Ddr.Integration
{
    [TestFixture]
    public class Ddr573AudioIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [Explicit("wip")]
        public void Test1()
        {
            var data = GetArchiveResource($"Ddr.mp3.zip")
                .First()
                .Value;
            var reader = Resolve<IDdr573AudioStreamReader>();

            var key = new byte[0x1E00];
            var scramble = new byte[0x1E00];
            var observed = reader.Read(new MemoryStream(data), data.Length, key, scramble, 1);

            File.WriteAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "573aud.bin"),
                observed);
        }
    }
}