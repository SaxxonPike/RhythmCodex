using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Psf.Streamers;

namespace RhythmCodex.Psf.Integration
{
    public class PsfIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [TestCase("ff9")]
        public void Test1(string name)
        {
            // Arrange.
            var data = GetArchiveResource($"Psf.{name}.zip")
                .First()
                .Value;

            var reader = Resolve<IPsfStreamReader>();
            var psf = reader.Read(new MemoryStream(data));
        }
    }
}