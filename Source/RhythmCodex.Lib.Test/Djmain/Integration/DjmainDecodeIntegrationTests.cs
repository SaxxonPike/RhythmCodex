using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Attributes;
using RhythmCodex.Audio.Converters;
using RhythmCodex.Audio.Streamers;
using RhythmCodex.Djmain.Converters;
using RhythmCodex.Djmain.Model;

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
        public void Test1()
        {
            var data = GetArchiveResource("Djmain.bmcm.zip")
                .First()
                .Value;

            var archive = DecodeChunk(data, DjmainChunkFormat.Complete);
            
            var sounds = archive.Samples.ToDictionary(
                s => $"{(int)s[NumericData.SampleMap]:00}_{(int)s[NumericData.Id]:0000}.wav", 
                Resolve<IRiffPcm16SoundEncoder>().Encode);

            foreach (var kv in sounds)
            {
                using (var stream =
                    new FileStream(Path.Combine(@"C:\Users\anthony.konzel\Desktop\UnitTest\out\", kv.Key),
                        FileMode.Create))
                {
                    var sound = kv.Value;
                    Resolve<IRiffStreamWriter>().Write(stream, sound);
                }
            }
        }
    }
}