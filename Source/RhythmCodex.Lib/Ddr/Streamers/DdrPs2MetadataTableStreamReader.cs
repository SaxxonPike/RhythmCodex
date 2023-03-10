using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Streamers
{
    [Service]
    public class DdrPs2MetadataTableStreamReader : IDdrPs2MetadataTableStreamReader
    {
        private const int BufferCheckInterval = 0x010;

        public IList<DdrPs2MetadataTableEntry> Get(Stream stream, long length)
        {
            var cache = new CachedStream(stream);
            var buffer = new byte[BufferCheckInterval];

            static bool IsName(ReadOnlySpan<byte> buff, int offs)
            {
                return buff[offs].IsLetterOrDigit() &&
                       buff[offs + 1].IsLetterOrDigit() &&
                       buff[offs + 2].IsLetterOrDigit() &&
                       buff[offs + 3].IsLetterOrDigit() &&
                       (buff[offs + 4] == 0 || buff[offs + 4].IsLetterOrDigit()) &&
                       buff[offs + 5] == 0;
            }

            static string GetName(ReadOnlySpan<byte> buff, int offs)
            {
                var name = buff.Slice(offs, 6).ToArray().TakeWhile(c => c != 0).ToArray().GetString();
                return name;
            }

            while (true)
            {
                if (cache.TryRead(buffer, 0, BufferCheckInterval) < BufferCheckInterval)
                    break;

                var recordSize = -1;
                if (IsName(buffer, 0x000) && DdrConstants.KnownSongNames.Contains(GetName(buffer, 0x000)))
                {
                    for (var i = 0x010; i < 0x200; i += 0x004)
                    {
                        cache.Position = i;
                        cache.TryRead(buffer, 0, 8);
                        if (IsName(buffer, 0) && DdrConstants.KnownSongNames.Contains(GetName(buffer, 0)))
                        {
                            recordSize = i;
                            break;
                        }
                    }
                }

                if (recordSize >= 100)
                {
                    cache.Rewind();
                    var records = new List<byte[]>();
                    while (true)
                    {
                        var record = new byte[recordSize];
                        cache.TryRead(record, 0, recordSize);

                        if (record[0] == 0 && record[1] == 0 && record[2] == 0 && record[3] == 0 && record[4] == 0 &&
                            record[5] == 0)
                            return records.Select((e, i) => new DdrPs2MetadataTableEntry
                            {
                                Data = e,
                                Index = i
                            }).ToList();

                        if (IsName(record, 0x000) && DdrConstants.KnownSongNames.Contains(GetName(record, 0x000)))
                        {
                            records.Add(record);
                            continue;
                        }

                        break;
                    }
                }
                
                cache.Advance(BufferCheckInterval);
                cache.Rewind();
            }

            return null;
        }
    }
}