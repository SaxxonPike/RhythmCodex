using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Attributes;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Xact.Converters;
using RhythmCodex.Xact.Streamers;

namespace RhythmCodex.Xact.Integration
{
    [TestFixture]
    public class XactIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [Explicit]
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
    }
}