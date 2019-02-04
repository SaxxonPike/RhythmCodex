using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers
{
    [Service]
    public class ChunkStreamReader : IChunkStreamReader
    {
        public SsqChunk Read(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var length = reader.ReadInt32();

            if (length == 0)
                return null;

            return new SsqChunk
            {
                Parameter0 = reader.ReadInt16(),
                Parameter1 = reader.ReadInt16(),
                Data = reader.ReadBytes(length - 8)
            };
        }
    }
}