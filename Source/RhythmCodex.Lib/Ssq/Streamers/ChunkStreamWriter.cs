using System;
using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers
{
    [Service]
    public class ChunkStreamWriter : IChunkStreamWriter
    {
        public void Write(Stream stream, SsqChunk ssqChunk)
        {
            var writer = new BinaryWriter(stream);

            if (ssqChunk == null)
            {
                writer.Write(0);
                return;
            }

            var chunkData = ssqChunk.Data ?? new byte[0];
            var writeLength = ((chunkData.Length + 3) >> 2) << 2;
            var writeData = new byte[writeLength];
            Array.Copy(chunkData, writeData, chunkData.Length);

            var parameter0 = ssqChunk.Parameter0;
            var parameter1 = ssqChunk.Parameter1;

            writer.Write(writeLength + 8);
            writer.Write(parameter0);
            writer.Write(parameter1);
            writer.Write(writeData);
        }
    }
}