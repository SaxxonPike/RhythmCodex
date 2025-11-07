using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Charts.Ssq.Streamers;

[Service]
public class SsqStreamReader(IChunkStreamReader chunkStreamReader) : ISsqStreamReader
{
    public List<SsqChunk> Read(Stream stream)
    {
        return ReadInternal(stream).ToList();
    }

    private IEnumerable<SsqChunk> ReadInternal(Stream stream)
    {
        while (true)
        {
            var chunk = chunkStreamReader.Read(stream);
            if (chunk == null)
                yield break;

            yield return chunk;
        }
    }
}