using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Arc.Converters;
using RhythmCodex.Arc.Streamers;

namespace RhythmCodex.Arc.Integration
{
    [TestFixture]
    public class ArcIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        public void LoadAndDecompress()
        {
            var reader = Resolve<IArcStreamReader>();
            var converter = Resolve<IArcFileConverter>();
            var archive = GetArchiveResource("Arc.ddra-arc.zip");
            var data = archive["test.arc"];
            var expected = archive["expected.dds"];
            using var stream = new MemoryStream(data);

            var output = reader
                .Read(stream)
                .Select(converter.Decompress)
                .ToList();

            var file = output[1];
            file.Name.Should().Be("data/chara/pl_shadow00/pl_shadow00.dds");
            file.Data.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ReadAndWriteGenerateSameFile()
        {
            var reader = Resolve<IArcStreamReader>();
            var writer = Resolve<IArcStreamWriter>();
            var archive = GetArchiveResource("Arc.ddra-arc.zip");
            var data = archive["test.arc"];
            using var inStream = new MemoryStream(data);
            using var outStream = new MemoryStream();

            var files = reader
                .Read(inStream)
                .ToList();
            
            writer.Write(outStream, files);
            outStream.Flush();

            this.WriteFile(outStream.ToArray(), "out.arc");
            
            inStream.ToArray().Should().BeEquivalentTo(outStream.ToArray());
        }

        [Test]
        [Explicit("this writes to the desktop, don't bother with this one")]
        public void DecryptTheWholeDamnUniverse()
        {
            var reader = Resolve<IArcStreamReader>();
            var converter = Resolve<IArcFileConverter>();
            var fileNames = Directory.GetFiles(@"\\tamarat\ddr\MDX-001-2018102200\contents", "*.arc", SearchOption.AllDirectories);

            foreach (var fileName in fileNames)
            {
                var data = File.ReadAllBytes(fileName);
                using var stream = new MemoryStream(data);
                
                var output = reader
                    .Read(stream)
                    .Select(converter.Decompress)
                    .ToList();

                foreach (var file in output)
                    this.WriteFile(file.Data, Path.Combine("arc", file.Name));
            }
        }
    }
}