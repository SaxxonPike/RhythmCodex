using System.Collections.Generic;
using RhythmCodex.Compression;
using RhythmCodex.Ddr.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Ps2.Converters;

[Service]
public class DdrPs2FileDataBoundTableDecoder(IBemaniLzDecoder bemaniLzDecoder) : IDdrPs2FileDataBoundTableDecoder
{
    public List<DdrPs2FileDataTableEntry> Decode(DdrPs2FileDataTableChunk chunk)
    {
        var data = chunk.Data;
        var result = new List<DdrPs2FileDataTableEntry>();
        var stream = new ReadOnlyMemoryStream(data);
        var offsets = Bitter.ToInt32Values(data.Span);
        var count = offsets[0];

        for (var i = 0; i < count; i++)
        {
            var offset = offsets[i + 1];
            var length = offsets[i + 1 + count];
            stream.Position = offset;
            result.Add(new DdrPs2FileDataTableEntry
            {
                Index = i,
                Data = bemaniLzDecoder.Decode(stream)
            });
        }

        return result;
    }
}