using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Xact.Converters;
using RhythmCodex.Xact.Heuristics;
using RhythmCodex.Xact.Streamers;

namespace RhythmCodex.Xact.Integration
{
    [TestFixture]
    public class XactIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [Explicit]
        public void Test_XSB()
        {
            // Arrange.
            var data = GetArchiveResource($"Xact.xsb.zip")
                .First()
                .Value;
            var reader = Resolve<IXsbStreamReader>();

            // Act.
            using var observed = new MemoryStream(data);
            var info = reader.Read(observed, observed.Length);
        }

        [Test]
        [Explicit]
        public void Test_XSB_Big()
        {
            // Arrange.
            var data = File.ReadAllBytes("Z:\\Mount\\DDR\\MDX-001-2019042200\\contents\\data\\sound\\win\\dance\\aaaa.xsb");
            var reader = Resolve<IXsbStreamReader>();

            // Act.
            using var observed = new MemoryStream(data);
            var info = reader.Read(observed, observed.Length);
        }
        
        [Test]
        [Explicit("writes to the desktop")]
        public void Test_XWB()
        {
            // Arrange.
            var data = GetArchiveResource($"Xact.xwb.zip")
                .First()
                .Value;
            var reader = Resolve<IXwbStreamReader>();
            var decoder = Resolve<IXwbDecoder>();
            var encoder = Resolve<IRiffPcm16SoundEncoder>();
            var writer = Resolve<IRiffStreamWriter>();

            // Act.
            using (var observed = new MemoryStream(data))
            {
                // Assert.
                foreach (var sound in reader.Read(observed))
                {
                    var decoded = decoder.Decode(sound);
                    var encoded = encoder.Encode(decoded);
                    var outfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "xwb");
                    if (!Directory.Exists(outfolder))
                        Directory.CreateDirectory(outfolder);

                    using (var outStream = new MemoryStream())
                    {
                        writer.Write(outStream, encoded);
                        outStream.Flush();
                        File.WriteAllBytes(Path.Combine(outfolder, $"{decoded[StringData.Name]}.wav"),
                            outStream.ToArray());
                    }
                }
            }
        }

        [Test]
        [Explicit("long test written for a blanket test, don't run this one")]
        public void TestDdraXwb()
        {
            var fileNames = Directory.GetFiles(@"\\tamarat\ddr\MDX-001-2018102200\contents", "*.xwb", SearchOption.AllDirectories);
            var reader = Resolve<IXwbStreamReader>();
            var decoder = Resolve<IXwbDecoder>();
            var encoder = Resolve<IRiffPcm16SoundEncoder>();
            var writer = Resolve<IRiffStreamWriter>();
            var heuristicTester = Resolve<IHeuristicTester>();

            foreach (var fileName in fileNames)
            {
                var outPath = Path.Combine(Paths.GetParts(fileName).Skip(4).ToArray());
                var rawBytes = File.ReadAllBytes(fileName);

                if (!heuristicTester.Match(rawBytes).Any(x => x.Heuristic is XwbHeuristic))
                    continue;
                
                using var observed = new MemoryStream(rawBytes);
                
                
                // Assert.
                foreach (var sound in reader.Read(observed))
                {
                    var decoded = decoder.Decode(sound);
                    var encoded = encoder.Encode(decoded);
                    var outfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "xwb", outPath);
                    if (!Directory.Exists(outfolder))
                        Directory.CreateDirectory(outfolder);

                    using (var outStream = new MemoryStream())
                    {
                        writer.Write(outStream, encoded);
                        outStream.Flush();
                        File.WriteAllBytes(Path.Combine(outfolder, $"{decoded[StringData.Name]}.wav"),
                            outStream.ToArray());
                    }
                }
            }
        }
    }
}