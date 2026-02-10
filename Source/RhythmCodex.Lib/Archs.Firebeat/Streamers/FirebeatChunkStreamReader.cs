using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Firebeat.Models;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Firebeat.Streamers;

[Service]
public class FirebeatChunkStreamReader : IFirebeatChunkStreamReader
{
    public const int ChunkSize = 0x2000000;

    public IEnumerable<FirebeatChunk> Read(Stream stream)
    {
        var id = 0;
        var data = new byte[ChunkSize];

        while (true)
        {
            var thisId = id++;

            if (stream.ReadAtLeast(data, ChunkSize, false) < ChunkSize)
                yield break;

            data.AsSpan().Swap16();
            
            yield return new FirebeatChunk
            {
                Format = FirebeatChunkFormat.Unknown,
                Data = data,
                Id = thisId
            };

            data = new byte[ChunkSize];
        }
    }
}