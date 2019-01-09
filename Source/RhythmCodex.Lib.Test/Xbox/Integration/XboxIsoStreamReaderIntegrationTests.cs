using System.IO;
using NUnit.Framework;
using RhythmCodex.Xbox.Streamers;

namespace RhythmCodex.Xbox.Integration
{
    [TestFixture]
    public class XboxIsoStreamReaderIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [TestCase(@"C:\Users\Saxxon\Desktop\Dance Dance Revolution Ultramix.iso")]
        [Explicit]
        public void Test1(string inputPath)
        {
            var reader = Resolve<IXboxIsoStreamReader>();
            
            using (var stream = File.OpenRead(inputPath))
            {
                var filesystem = reader.Read(stream, stream.Length);
            }
        }
    }
}