using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Iso.Converters;
using RhythmCodex.Iso.Streamers;

namespace RhythmCodex.Iso.Integration
{
    [TestFixture]
    public class IsoIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [Explicit]
        public void TestBoringIso()
        {
            var data = GetArchiveResource($"Iso.test-iso.zip")
                .First()
                .Value;
            var mem = new MemoryStream(data);
            
            var reader = Resolve<IIsoSectorStreamReader>();
            var decoder = Resolve<IIsoSectorInfoDecoder>();
            var storageDecoder = Resolve<IIsoStorageMediumDecoder>();
            
            var sectors = reader.Read(mem, (int) mem.Length, false).ToList();
            var infos = sectors.Select(decoder.Decode).ToArray();
            var decoded = storageDecoder.Decode(infos);
        }
    }
}