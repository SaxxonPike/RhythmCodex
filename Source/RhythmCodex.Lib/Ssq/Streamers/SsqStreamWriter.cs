using System.Collections.Generic;
using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers;

[Service]
public class SsqStreamWriter(IChunkStreamWriter chunkStreamWriter) 
    : ISsqStreamWriter
{
    public void Write(Stream stream, IEnumerable<SsqChunk> chunks)
    {
        foreach (var chunk in chunks)
            chunkStreamWriter.Write(stream, chunk);

        chunkStreamWriter.WriteEnd(stream);
    }
}