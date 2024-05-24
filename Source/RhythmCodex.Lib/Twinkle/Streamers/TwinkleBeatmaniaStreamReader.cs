using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Streamers;

[Service]
public class TwinkleBeatmaniaStreamReader : ITwinkleBeatmaniaStreamReader
{
    private const int ChunkLength = 0x1A00000;
    private const int DataStart = 0x8000000;

    public IEnumerable<TwinkleBeatmaniaChunk> Read(Stream stream, long length, bool skipHeader = true)
    {
        var index = 0;
        var reader = new BinaryReader(stream);
        var actualLength = length / ChunkLength * ChunkLength;

        if (skipHeader)
            stream.SkipBytes(DataStart);

        var offset = 0L;
        while (offset < actualLength)
        {
            var data = reader.ReadBytes(ChunkLength);
            data.AsSpan().Swap16();

            yield return new TwinkleBeatmaniaChunk
            {
                Data = data,
                Index = index,
                Offset = offset
            };

            offset += ChunkLength;
            index++;
        }
    }
}