using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Twinkle.Model;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Twinkle.Streamers;

[Service]
public class TwinkleBeatmaniaStreamReader : ITwinkleBeatmaniaStreamReader
{
    private const int ChunkSize = TwinkleConstants.ChunkSize;
    private const int DataStart = 0x8000000;

    public IEnumerable<TwinkleBeatmaniaChunk> Read(Stream stream, long length, bool skipHeader = true)
    {
        var index = 0;
        var reader = new BinaryReader(stream);
        var actualLength = length / ChunkSize * ChunkSize;

        if (skipHeader)
            stream.SkipBytes(DataStart);

        var offset = 0L;
        while (offset < actualLength)
        {
            var data = reader.ReadBytes(ChunkSize);
            data.AsSpan().Swap16();

            yield return new TwinkleBeatmaniaChunk
            {
                Data = data,
                Index = index,
                Offset = offset
            };

            offset += ChunkSize;
            index++;
        }
    }
}