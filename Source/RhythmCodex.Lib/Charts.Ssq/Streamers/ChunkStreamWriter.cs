using System.IO;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Charts.Ssq.Streamers;

[Service]
public class ChunkStreamWriter : IChunkStreamWriter
{
    public void Write(Stream stream, SsqChunk ssqChunk)
    {
        var writer = new BinaryWriter(stream);

        var chunkData = ssqChunk.Data;
        var writeLength = ((chunkData.Length + 3) >> 2) << 2;
        var writeData = new byte[writeLength];
        chunkData.CopyTo(writeData);

        var parameter0 = ssqChunk.Parameter0;
        var parameter1 = ssqChunk.Parameter1;

        writer.Write(writeLength + 8);
        writer.Write(parameter0);
        writer.Write(parameter1);
        writer.Write(writeData);
    }

    public void WriteEnd(Stream stream)
    {
        var writer = new BinaryWriter(stream);
        writer.Write(0);
    }
}