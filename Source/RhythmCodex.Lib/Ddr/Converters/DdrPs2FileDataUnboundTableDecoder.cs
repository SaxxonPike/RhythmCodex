using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using RhythmCodex.Compression;
using RhythmCodex.Ddr.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Converters
{
    [Service]
    public class DdrPs2FileDataUnboundTableDecoder : IDdrPs2FileDataUnboundTableDecoder
    {
        private readonly IBemaniLzDecoder _bemaniLzDecoder;

        public DdrPs2FileDataUnboundTableDecoder(IBemaniLzDecoder bemaniLzDecoder)
        {
            _bemaniLzDecoder = bemaniLzDecoder;
        }
        
        public IList<DdrPs2FileDataTableEntry> Decode(DdrPs2FileDataTableChunk chunk)
        {
            var data = chunk.Data;
            var result = new List<DdrPs2FileDataTableEntry>();
            var stream = new MemoryStream(data);
            var offsets = MemoryMarshal.Cast<byte, int>(data.AsSpan(0, data.Length / 4 * 4));
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
                    Data = _bemaniLzDecoder.Decode(stream)
                });
            }

            return result;
        }
    }
}