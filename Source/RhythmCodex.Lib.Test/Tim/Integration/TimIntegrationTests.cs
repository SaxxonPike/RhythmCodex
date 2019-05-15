using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Tim.Converters;

namespace RhythmCodex.Tim.Integration
{
    public class TimIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [TestCase("extreme1")]
        public void Test1(string name)
        {
            // Arrange.
            var data = GetArchiveResource($"Tim.{name}.zip")
                .First()
                .Value;

            var decoder = Resolve<ITimDecoder>();

            // Act.
            using (var stream = new MemoryStream(data))
            {
                var output = decoder.Decode(stream);
            }
        }
    }
}