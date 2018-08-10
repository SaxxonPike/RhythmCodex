using System.Linq;
using NUnit.Framework;
using RhythmCodex.Djmain.Converters;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Integration
{
    [TestFixture]
    public class DjmainDecodeIntegrationTests : BaseIntegrationFixture<DjmainDecoder>
    {
        private IDjmainArchive DecodeChunk(byte[] data, DjmainChunkFormat format)
        {
            return Subject.Decode(new DjmainChunk
            {
                Data = data,
                Format = format
            });
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