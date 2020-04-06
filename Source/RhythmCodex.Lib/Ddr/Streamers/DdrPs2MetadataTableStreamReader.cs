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
        private const int BufferSize = 0x200;
        private const int BufferCheckInterval = 0x010;

        public IList<DdrPs2MetadataTableEntry> Get(Stream stream, long length)
        {
            var buffer = new byte[BufferSize];
            if (stream.TryRead(buffer, 0, BufferSize) < BufferSize)
                throw new Exception("Can't read enough bytes into the buffer");
            var offset = BufferSize;

            bool IsName(byte[] buff, int offs)
            {
                return buff[offs].IsLetter() &&
                       buff[offs + 1].IsLetter() &&
                       buff[offs + 2].IsLetter() &&
                       buff[offs + 3].IsLetterOrDigit() &&
                       (buff[offs + 4] == 0 || buff[offs + 4].IsLetterOrDigit()) &&
                       buff[offs + 5] == 0;
            }

            string GetName(byte[] buff, int offs)
            {
                return buff.AsSpan(offs, 6).ToArray().TakeWhile(c => c != 0).ToArray().GetString();
            }

            while (offset < length)
            {
                var recordSize = -1;
                if (IsName(buffer, 0x000) && DdrConstants.KnownSongNames.Contains(GetName(buffer, 0x000)))
                {
                    for (var i = 0x008; i < BufferSize; i += 0x004)
                    {
                        if (IsName(buffer, i) && DdrConstants.KnownSongNames.Contains(GetName(buffer, i)))
                        {
                            recordSize = i;
                            break;
                        }
                    }
                }

                if (recordSize >= 100)
                {
                    var bufferRemainder = BufferSize;
                    var bufferOffset = 0x000;
                    var records = new List<byte[]>();
                    while (true)
                    {
                        var record = new byte[recordSize];

                        if (bufferRemainder > recordSize)
                        {
                            buffer.AsSpan(bufferOffset, recordSize).CopyTo(record);
                            bufferOffset += recordSize;
                            bufferRemainder -= recordSize;
                        }
                        else if (bufferRemainder > 0)
                        {
                            buffer.AsSpan(bufferOffset, bufferRemainder).CopyTo(record);
                            if (bufferRemainder < recordSize)
                                stream.TryRead(record, bufferRemainder, recordSize - bufferRemainder);
                            bufferRemainder = 0;
                        }
                        else
                        {
                            stream.TryRead(record, 0, recordSize);
                        }

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

                buffer.AsSpan(BufferCheckInterval, BufferSize - BufferCheckInterval).CopyTo(buffer);
                if (stream.TryRead(buffer, BufferSize - BufferCheckInterval, BufferCheckInterval) < BufferCheckInterval)
                    throw new Exception("Can't read enough bytes into rotating buffer");
                offset += BufferCheckInterval;
            }

            return null;
        }
    }
}