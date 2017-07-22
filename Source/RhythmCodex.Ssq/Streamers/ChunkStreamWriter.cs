using System.IO;
using RhythmCodex.Ssq.Model;
using RhythmCodex.Streamers;

namespace RhythmCodex.Ssq.Streamers
{
    public class ChunkStreamWriter : IStreamWriter<Chunk?>
    {
        public void Write(Stream stream, Chunk? chunk)
        {
            var writer = new BinaryWriter(stream);

            if (chunk == null)
            {
                writer.Write(0);
                return;
            }

            var value = chunk.Value;

            var parameter0 = value.Parameter0;
            var parameter1 = value.Parameter1;
            var length = value.Data?.Length ?? 0;

            writer.Write(length + 8);
            writer.Write(parameter0);
            writer.Write(parameter1);

            if (value.Data != null)
                writer.Write(value.Data);
        }
    }
}
