using System.IO;
using RhythmCodex.Ssq.Model;
using RhythmCodex.Streamers;

namespace RhythmCodex.Ssq.Streamers
{
    public class ChunkStreamer : IStreamer<IChunk, IChunk>
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

        public void Write(Stream stream, IChunk chunk)
        {
            var writer = new BinaryWriter(stream);

            if (chunk == null)
            {
                writer.Write(0);
                return;
            }

            var parameter0 = (short)chunk.Parameter0;
            var parameter1 = (short)chunk.Parameter1;
            var length = chunk.Data?.Length ?? 0;

            writer.Write(length + 8);
            writer.Write(parameter0);
            writer.Write(parameter1);
            
            if (chunk.Data != null)
                writer.Write(chunk.Data);
        }
    }
}
