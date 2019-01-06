using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Xact.Streamers;

namespace RhythmCodex.Xact.Integration
{
    [TestFixture]
    public class XactIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        public void Test1()
        {
            // Arrange.
//            var data = GetArchiveResource($"Xact.xwb.zip")
//                .First()
//                .Value;
            var reader = Resolve<IXwbStreamReader>();
            var encoder = Resolve<IRiffPcm16SoundEncoder>();
            var writer = Resolve<IRiffStreamWriter>();

            // Act.
//            var observed = reader.Read(new MemoryStream(data));
            using (var observed = File.OpenRead(@"C:\Users\Saxxon\Desktop\bgm_dance.xwb"))
            {
                // Assert.
                var index = 0;
                foreach (var sound in reader.Read(observed))
                {
                    var encoded = encoder.Encode(sound);
                    var outfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "xwb");
                    if (!Directory.Exists(outfolder))
                        Directory.CreateDirectory(outfolder);

                    using (var outStream = new MemoryStream())
                    {
                        writer.Write(outStream, encoded);
                        outStream.Flush();
                        File.WriteAllBytes(Path.Combine(outfolder, $"{index:000}.wav"), outStream.ToArray());
                        index++;
                    }
                }
            }
        }
    }
}