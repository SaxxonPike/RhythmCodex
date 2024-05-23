using System.Collections.Generic;
using System.IO;
using RhythmCodex.Compression;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Converters;

[Service]
public class DdrPs2FileDataUnboundTableDecoder(IBemaniLzDecoder bemaniLzDecoder) : IDdrPs2FileDataUnboundTableDecoder
{
    public IList<DdrPs2FileDataTableEntry> Decode(DdrPs2FileDataTableChunk? chunk)
    {
        if (chunk == null)
            return [];
        
        var data = chunk.Data;
        var result = new List<DdrPs2FileDataTableEntry>();
        var stream = new MemoryStream(data);
        var offsets = Bitter.ToInt32Values(data);
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