using System;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Converters
{
    [Service]
    public class DdrPs2DatabaseDecoder : IDdrPs2DatabaseDecoder
    {
        public DdrDatabaseEntry Decode(DdrPs2MetadataTableEntry item)
        {
            if (item.Data.Length < 120)
                return GetOldRecord(item);
            else
                return GetNewRecord(item);
        }

        private static DdrDatabaseEntry GetOldRecord(DdrPs2MetadataTableEntry item)
        {
            var record = item.Data.AsSpan();
            var id = Encodings.CP437.GetStringWithoutNulls(record.Slice(0x00, 5));
            if (id == string.Empty)
                return null;

            return new DdrDatabaseEntry
            {
                Index = item.Index,
                Id = id,
                Type = record[0x06],
                CdTitle = record[0x07],
                InternalId = Bitter.ToInt16(record, 0x08),
                MaxBpm = Bitter.ToInt16(record, 0x10),
                MinBpm = Bitter.ToInt16(record, 0x12),
                Unknown014 = Bitter.ToInt16(record, 0x14),
                SonglistOrder = Bitter.ToInt16(record, 0x16),
                UnlockNumber = Bitter.ToInt16(record, 0x18),
                Difficulties = new int[0],
                Flags = Bitter.ToInt32(record, 0x1C),
                // Radar0 = Bitter.ToInt16Array(record, 0x20, 6),
                // Radar1 = Bitter.ToInt16Array(record, 0x2C, 6),
                // Radar2 = Bitter.ToInt16Array(record, 0x38, 6),
                // Radar3 = Bitter.ToInt16Array(record, 0x44, 6),
                // Radar4 = Bitter.ToInt16Array(record, 0x50, 6)
            };
        }

        private static DdrDatabaseEntry GetNewRecord(DdrPs2MetadataTableEntry records)
        {
            var record = records.Data.AsSpan();
            var id = Encodings.CP437.GetStringWithoutNulls(record.Slice(0x00, 5));
            if (id == string.Empty)
                return null;

            var bpmOffset = 0;
            if (Bitter.ToInt32(record, 0x14) != -1)
            {
                while (Bitter.ToInt32(record, 0x10 + bpmOffset) == 0)
                    bpmOffset += 4;
            }

            var difficultyOffset = bpmOffset;
            if (bpmOffset > 0)
            {
                while (Bitter.ToInt32(record, 0x24 + difficultyOffset) == 0)
                    difficultyOffset += 4;
            }

            return new DdrDatabaseEntry
            {
                Index = records.Index,
                Id = id,
                Type = record[0x06],
                CdTitle = record[0x07],
                InternalId = Bitter.ToInt16(record, 0x08),
                MaxBpm = Bitter.ToInt16(record, 0x10 + bpmOffset),
                MinBpm = Bitter.ToInt16(record, 0x12 + bpmOffset),
                Unknown014 = Bitter.ToInt16(record, 0x14 + bpmOffset),
                SonglistOrder = Bitter.ToInt16(record, 0x16 + bpmOffset),
                UnlockNumber = Bitter.ToInt16(record, 0x18 + bpmOffset),
                Difficulties = new[]
                {
                    record[0x24 + difficultyOffset] & 0xF,
                    record[0x24 + difficultyOffset] >> 4,
                    record[0x25 + difficultyOffset] & 0xF,
                    record[0x25 + difficultyOffset] >> 4,
                    record[0x26 + difficultyOffset] & 0xF,
                    record[0x26 + difficultyOffset] >> 4,
                    record[0x27 + difficultyOffset] & 0xF,
                    record[0x27 + difficultyOffset] >> 4,
                    record[0x28 + difficultyOffset] & 0xF,
                    record[0x28 + difficultyOffset] >> 4,
                    record[0x29 + difficultyOffset] & 0xF,
                    record[0x29 + difficultyOffset] >> 4,
                    record[0x2A + difficultyOffset] & 0xF,
                    record[0x2A + difficultyOffset] >> 4,
                    record[0x2B + difficultyOffset] & 0xF,
                    record[0x2B + difficultyOffset] >> 4
                },
                Flags = Bitter.ToInt32(record, 0x2C + difficultyOffset),
                // Radar0 = Bitter.ToInt16Array(record, 0x30, 6),
                // Radar1 = Bitter.ToInt16Array(record, 0x3C, 6),
                // Radar2 = Bitter.ToInt16Array(record, 0x48, 6),
                // Radar3 = Bitter.ToInt16Array(record, 0x54, 6),
                // Radar4 = Bitter.ToInt16Array(record, 0x60, 6)
            };
        }
    }
}