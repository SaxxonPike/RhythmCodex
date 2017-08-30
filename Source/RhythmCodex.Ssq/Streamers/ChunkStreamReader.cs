using System.IO;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers
{
    public class ChunkStreamReader : IChunkStreamReader
    {
        public IChunk Read(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var length = reader.ReadInt32();

            if (length == 0)
                return null;
            
            return new Chunk
            {
                Parameter0 = reader.ReadInt16(),
                Parameter1 = reader.ReadInt16(),
                Data = reader.ReadBytes(length - 8)
            };
        }
    }
}
