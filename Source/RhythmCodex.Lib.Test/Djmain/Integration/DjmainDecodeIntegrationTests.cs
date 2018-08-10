using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Charting;
using RhythmCodex.Djmain.Converters;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Djmain.Streamers;
using RhythmCodex.Streamers;

namespace RhythmCodex.Djmain.Integration
{
    [TestFixture]
    public class DjmainDecodeIntegrationTests : BaseIntegrationFixture<DjmainDecoder>
    {
        private IDjmainArchive DecodeChunk(byte[] data, DjmainChunkFormat format)
        {
            var decoder = Resolve<DjmainDecoder>();

            using (var mem = new ByteSwappedReadStream(new MemoryStream(data)))
            using (var reader = new BinaryReader(mem))
            {
                return Subject.Decode(new DjmainChunk
                {
                    Data = reader.ReadBytes((int)mem.Length),
                    Format = format
                });
            }
        }

        [Test]
        public void Test1()
        {
            var data = GetArchiveResource("Djmain.bm1st.zip")
                .First()
                .Value;

            var archive = DecodeChunk(data, DjmainChunkFormat.First);
        }
    }
}