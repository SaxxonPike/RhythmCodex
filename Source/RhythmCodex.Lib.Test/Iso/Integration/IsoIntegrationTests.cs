using System;
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
            var decoder = Resolve<IIsoCdFileDecoder>();

            var sectors = reader.Read(mem, (int) mem.Length, true);
            var files = decoder.Decode(sectors);
            var file = files.First();
            using var stream = file.Open();
            var fileReader = new BinaryReader(stream);
            var output = fileReader.ReadBytes((int) file.Length);
            File.WriteAllBytes(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    Path.GetFileName(file.Name)), output);
        }
    }
}