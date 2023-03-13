using System;
using System.Collections.Generic;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Converters
{
    [Service]
    public class Ddr573DatabaseDecoder : IDdr573DatabaseDecoder
    {
        public IList<DdrDatabaseEntry> Decode(ReadOnlySpan<byte> database)
        {
            var offset = 0;
            var length = database.Length;
            var result = new List<DdrDatabaseEntry>();
            var shortNameOffsets = new Dictionary<int, int>();
            var longNameOffsets = new Dictionary<int, int>();
            var index = 0;

            // Read database entries
            while (offset < length)
            {
                var raw = database.Slice(offset, 0x80);
                var id = Encodings.CP437.GetStringWithoutNulls(raw.Slice(0x00, 5));
                if (id == string.Empty)
                    break;

                var entry = new DdrDatabaseEntry
                {
                    Index = index,
                    Id = id,
                    Type = raw[0x06],
                    CdTitle = raw[0x07],
                    InternalId = Bitter.ToInt16(raw, 0x08),
                    MaxBpm = Bitter.ToInt16(raw, 0x10),
                    MinBpm = Bitter.ToInt16(raw, 0x12),
                    Unknown014 = Bitter.ToInt16(raw, 0x14),
                    SonglistOrder = Bitter.ToInt16(raw, 0x16),
                    UnlockNumber = Bitter.ToInt16(raw, 0x18),
                    Difficulties = new[]
                    {
                        raw[0x1C] & 0xF,
                        raw[0x1C] >> 4,
                        raw[0x1D] & 0xF,
                        raw[0x1D] >> 4,
                        raw[0x20] & 0xF,
                        raw[0x20] >> 4,
                        raw[0x21] & 0xF,
                        raw[0x21] >> 4,
                    },
                    Unknown01E = Bitter.ToInt16(raw, 0x1E),
                    Unknown022 = Bitter.ToInt16(raw, 0x22),
                    Flags = Bitter.ToInt32(raw, 0x24),
                    Radar0 = Bitter.ToInt16Values(raw, 0x28, 8),
                    Radar1 = Bitter.ToInt16Values(raw, 0x38, 8),
                    Radar2 = Bitter.ToInt16Values(raw, 0x48, 8),
                    Radar3 = Bitter.ToInt16Values(raw, 0x58, 8),
                    Radar4 = Bitter.ToInt16Values(raw, 0x68, 8)
                };

                longNameOffsets[index] = Bitter.ToInt32(raw, 0x78);
                shortNameOffsets[index] = Bitter.ToInt32(raw, 0x7C);

                result.Add(entry);
                offset += 0x80;
                index++;
            }

            offset += 0x80;

            // Read string table
            var strings = database.Slice(offset);
            foreach (var kv in longNameOffsets)
                result[kv.Key].LongName = Encodings.CP437.GetStringWithoutNulls(strings.Slice(kv.Value));

            foreach (var kv in shortNameOffsets)
                result[kv.Key].ShortName = Encodings.CP437.GetStringWithoutNulls(strings.Slice(kv.Value));

            return result;
        }

        public int FindRecordSize(ReadOnlySpan<byte> database)
        {
            for (var size = 16; size < 256; size++)
            {
                var fail = false;
                for (var i = 0; i < 16; i++)
                {
                    var test = database.Slice(size * i);
                    if (!test[0].IsLetter())
                    {
                        fail = true;
                        break;
                    }

                    if (!test[1].IsLetter())
                    {
                        fail = true;
                        break;
                    }

                    if (!test[2].IsLetter())
                    {
                        fail = true;
                        break;
                    }

                    if (!test[3].IsLetterOrDigit())
                    {
                        fail = true;
                        break;
                    }

                    if (!test[4].IsLetterOrDigit() && test[4] != 0)
                    {
                        fail = true;
                        break;
                    }

                    if (test[5] != 0)
                    {
                        fail = true;
                        break;
                    }
                }

                if (fail)
                    continue;

                return size;
            }

            throw new RhythmCodexException("Can't seem to find record size for MDB");
        }
    }
}