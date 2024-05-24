﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers;

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