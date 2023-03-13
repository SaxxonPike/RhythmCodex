using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Xbox.Streamers;

namespace RhythmCodex.Xbox.Integration
{
    public class XboxKasStreamReaderIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [TestCase("Xbox.kas.zip")]
        [Explicit]
        public void Test_Xbox_Kas(string source)
        {
            var data = GetArchiveResource(source)
                .First()
                .Value;

            var stream = new MemoryStream(data);

            var reader = Resolve<IXboxKasStreamReader>();
            foreach (var entry in reader.Read(stream))
            {
                this.WriteFile(entry.Data, $"kas\\{entry.Offset:X8}.wav");
            }
        }

        // some ugly oneshot code.
        [Test]
        [TestCase(@"C:\Users\Saxxon\Desktop\ddriso\DDR UNIVERSE1 (XB360 NTSC-U)", "ddr-universe1")]
        [TestCase(@"C:\Users\Saxxon\Desktop\ddriso\DDR UNIVERSE2 (XB360 NTSC-U)", "ddr-universe2")]
        [TestCase(@"C:\Users\Saxxon\Desktop\ddriso\DDR UNIVERSE3 (XB360 NTSC-U)", "ddr-universe3")]
        [Explicit]
        public void Test_Xbox_Kas_All(string inPath, string outPartial)
        {
            var outPath = Path.Combine(@"C:\Users\Saxxon\Desktop\ddriso\to-decode", outPartial);
            var reader = Resolve<IXboxKasStreamReader>();

            foreach (var fileName in Directory.GetFiles(inPath, "*.kas"))
            {
                using var inStream = File.OpenRead(fileName);
                var idx = 0;
                foreach (var entry in reader.Read(inStream))
                {
                    var outFile = $"{Path.GetFileNameWithoutExtension(fileName)}-{idx:D2}.xma";
                    var finalOutPath = Path.Combine(outPath, outFile);
                    File.WriteAllBytes(finalOutPath, entry.Data);
                    idx++;

                    var proc = new Process
                    {
                        StartInfo =
                        {
                            FileName = @"C:\Program Files (x86)\XMPlay\plugins\test.exe",
                            Arguments = $"\"{finalOutPath}\""
                        }
                    };

                    proc.Start();
                }
            }
        }
    }
}