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
    public class DdrPs2FileDataTableDecoder : IDdrPs2FileDataTableDecoder
    {
        private readonly IBemaniLzDecoder _bemaniLzDecoder;

        public DdrPs2FileDataTableDecoder(IBemaniLzDecoder bemaniLzDecoder)
        {
            _bemaniLzDecoder = bemaniLzDecoder;
        }
        
        public IList<DdrPs2FileDataTableEntry> Decode(byte[] data)
        {
            var result = new List<DdrPs2FileDataTableEntry>();
            var stream = new MemoryStream(data);
            var offsets = MemoryMarshal.Cast<byte, int>(data);
            var offsetCount = offsets.Length;
            for (var i = 0; i < offsetCount; i++)
            {
                var newCount = offsets[i] / 4;
                if (newCount < offsetCount)
                    offsetCount = newCount;
            }

            for (var i = 0; i < offsetCount; i++)
            {
                stream.Position = offsets[i];
                result.Add(new DdrPs2FileDataTableEntry
                {
                    Data = _bemaniLzDecoder.Decode(stream)
                });
            }

            return result;
        }
    }
}