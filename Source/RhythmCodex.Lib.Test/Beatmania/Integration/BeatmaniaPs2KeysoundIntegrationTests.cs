using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Beatmania.Converters;
using RhythmCodex.Beatmania.Streamers;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;

namespace RhythmCodex.Beatmania.Integration
{
    [TestFixture]
    public class BeatmaniaPs2KeysoundIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [Explicit]
        public void Test_Keys_Old()
        {
            var data = GetArchiveResource($"BeatmaniaPs2.bm2dxps2key.zip")
                .First()
                .Value;

            var streamer = Resolve<ips2str>();
            var decoder = Resolve<IBeatmaniaPcAudioDecoder>();
            var encoder = Resolve<IRiffPcm16SoundEncoder>();
            var writer = Resolve<IRiffStreamWriter>();

            var outFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "bmpc");
            if (!Directory.Exists(outFolder))
                Directory.CreateDirectory(outFolder);

            using (var dataStream = new MemoryStream(decrypter.Decrypt(new MemoryStream(data), data.Length)))
            {
                var keysounds = streamer.Read(dataStream, dataStream.Length);
                var index = 1;
                
                foreach (var keysound in keysounds)
                {
                    var decoded = decoder.Decode(keysound);
                    var encoded = encoder.Encode(decoded);
                    using (var outStream = new MemoryStream())
                    {
                        writer.Write(outStream, encoded);
                        outStream.Flush();
                        File.WriteAllBytes(Path.Combine(outFolder, $"{index:D4}.wav"), outStream.ToArray());
                        index++;
                    }
                }
            }
        }
    }
}