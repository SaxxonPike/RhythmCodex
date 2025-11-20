using System.Collections.Generic;
using System.IO;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Charts.Ssq.Streamers;

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