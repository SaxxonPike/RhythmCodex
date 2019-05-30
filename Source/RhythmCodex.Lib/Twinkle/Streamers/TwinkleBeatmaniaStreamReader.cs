using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Streamers
{
    [Service]
    public class TwinkleBeatmaniaStreamReader : ITwinkleBeatmaniaStreamReader
    {
        private const int ChunkLength = 0x1A00000;
        private const int DataStart = 0x8000000;

        public IEnumerable<TwinkleBeatmaniaChunk> Read(Stream stream, long length)
        {
            var index = 0;
            var reader = new BinaryReader(stream);
            stream.SkipBytes(DataStart);
            //stream.Seek(DataStart, SeekOrigin.Current);

            var offset = 0L;
            while (offset < length)
            {
                var data = reader.ReadBytes(ChunkLength);
                yield return new TwinkleBeatmaniaChunk
                {
                    Data = data,
                    Index = index,
                    Offset = offset
                };

                offset += ChunkLength;
                index++;
            }
        }
    }
}