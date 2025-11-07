using System.Collections.Generic;
using RhythmCodex.Compression;
using RhythmCodex.Ddr.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Ps2.Converters;

[Service]
public class DdrPs2FileDataUnboundTableDecoder(IBemaniLzDecoder bemaniLzDecoder) : IDdrPs2FileDataUnboundTableDecoder
{
    public List<DdrPs2FileDataTableEntry> Decode(DdrPs2FileDataTableChunk? chunk)
    {
        if (chunk == null)
            return [];
        
        var data = chunk.Data;
        var result = new List<DdrPs2FileDataTableEntry>();
        var stream = new ReadOnlyMemoryStream(data);
        var offsets = Bitter.ToInt32Values(data.Span);
        var offsetCount = offsets.Length;
        for (var i = 0; i < offsetCount; i++)
        {
            var newCount = offsets[i] / 4;
            if (newCount < offsetCount)
                offsetCount = newCount;
        }

        for (var i = 0; i < offsetCount; i++)
        {
            stream.Position = offsets[i] + (chunk.HasHeaders ? 0xC : 0x0);
                
            result.Add(new DdrPs2FileDataTableEntry
            {
                Index = i,
                Data = bemaniLzDecoder.Decode(stream)
            });
        }

        return result;
    }
}