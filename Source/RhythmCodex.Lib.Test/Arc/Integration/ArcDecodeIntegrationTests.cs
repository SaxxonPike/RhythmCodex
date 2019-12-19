using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Arc.Streamers;

namespace RhythmCodex.Arc.Integration
{
    [TestFixture]
    public class ArcDecodeIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        public void Test1()
        {
            var reader = Resolve<IArcStreamReader>();
            var data = GetArchiveResource("Arc.ddra-arc.zip").First().Value;
            using var stream = new MemoryStream(data);

            var output = reader.Read(stream).ToList();
        }
    }
}