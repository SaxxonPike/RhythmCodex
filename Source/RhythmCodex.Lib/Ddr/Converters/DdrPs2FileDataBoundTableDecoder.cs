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
    public class DdrPs2FileDataBoundTableDecoder : IDdrPs2FileDataBoundTableDecoder
    {
        private readonly IBemaniLzDecoder _bemaniLzDecoder;

        public DdrPs2FileDataBoundTableDecoder(IBemaniLzDecoder bemaniLzDecoder)
        {
            _bemaniLzDecoder = bemaniLzDecoder;
        }

        public IList<DdrPs2FileDataTableEntry> Decode(DdrPs2FileDataTableChunk chunk)
        {
            var data = chunk.Data;
            var result = new List<DdrPs2FileDataTableEntry>();
            var stream = new MemoryStream(data);
            var offsets = MemoryMarshal.Cast<byte, int>(data.AsSpan(0, data.Length / 4 * 4));
            var count = offsets[0];

            for (var i = 0; i < count; i++)
            {
                var offset = offsets[i + 1];
                var length = offsets[i + 1 + count];
                stream.Position = offset;
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