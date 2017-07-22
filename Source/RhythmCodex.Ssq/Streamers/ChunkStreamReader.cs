using System.IO;
using RhythmCodex.Ssq.Model;
using RhythmCodex.Streamers;

namespace RhythmCodex.Ssq.Streamers
{
    public class ChunkStreamReader : IStreamReader<Chunk?>
    {
        public Chunk? Read(Stream stream)
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
