using System;
using System.Collections.Generic;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Converters
{
    [Service]
    public class Ddr573DatabaseDecoder : IDdr573DatabaseDecoder
    {
        public IList<Ddr573DatabaseEntry> Decode(byte[] database)
        {
            var offset = 0;
            var length = database.Length;
            var result = new List<Ddr573DatabaseEntry>();

            // Read database entries
            while (offset < length)
            {
                var raw = database.AsSpan(offset, 0x80);
                var entry = new Ddr573DatabaseEntry
                {
                    Id = Encodings.CP437.GetStringWithoutNulls(raw.Slice(0, 5))
                };
                result.Add(entry);
                offset += 0x80;
            }
            
            // Read string table
            // TODO

            return result;
        }
    }
}