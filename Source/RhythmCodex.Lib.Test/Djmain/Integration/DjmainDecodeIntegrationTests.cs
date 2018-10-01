using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Attributes;
using RhythmCodex.Djmain.Converters;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;

namespace RhythmCodex.Djmain.Integration
{
    [TestFixture]
    public class DjmainDecodeIntegrationTests : BaseIntegrationFixture
    {
        private IDjmainArchive DecodeChunk(byte[] data, DjmainChunkFormat format)
        {
            return Resolve<IDjmainDecoder>().Decode(new DjmainChunk
            {
                Data = data,
                Format = format
            });
        }

        [Test]
        [Explicit]
        public void Test1()
        {
            var data = GetArchiveResource("Djmain.bmcm2.zip")
                .First()
                .Value;

            var archive = DecodeChunk(data, DjmainChunkFormat.Complete2);
            
            var sounds = archive.Samples.ToDictionary(
                s => $"{(int)s[NumericData.SampleMap]:00}_{(int)s[NumericData.Id]:0000}.wav", 
                Resolve<IRiffPcm16SoundEncoder>().Encode);

            var outPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "UnitTest", "out");
            Directory.CreateDirectory(outPath);
            
            foreach (var kv in sounds)
            {
                using (var stream =
                    new FileStream(Path.Combine(outPath, kv.Key),
                        FileMode.Create))
                {
                    var sound = kv.Value;
                    Resolve<IRiffStreamWriter>().Write(stream, sound);
                }
            }
        }
    }
}