using System;
using System.IO;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers
{
    public class ChunkStreamWriter : IChunkStreamWriter
    {
        public void Write(Stream stream, IChunk chunk)
        {
            var writer = new BinaryWriter(stream);
            
            if (chunk == null)
            {
                writer.Write(0);
                return;
            }

            var chunkData = chunk.Data ?? new byte[0];
            var writeLength = ((chunkData.Length + 3) >> 2) << 2;
            var writeData = new byte[writeLength];
            Array.Copy(chunkData, writeData, chunkData.Length);

            var parameter0 = chunk.Parameter0;
            var parameter1 = chunk.Parameter1;

            writer.Write(writeLength + 8);
            writer.Write(parameter0);
            writer.Write(parameter1);
            writer.Write(writeData);
        }
    }
}
