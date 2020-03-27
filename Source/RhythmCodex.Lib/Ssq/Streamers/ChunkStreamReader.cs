using System.IO;
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

            if (stream.CanSeek)
            {
                if (stream.Position >= stream.Length)
                    return null;
            }
            
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