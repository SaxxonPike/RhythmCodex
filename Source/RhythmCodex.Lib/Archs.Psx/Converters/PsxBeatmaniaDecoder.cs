using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.Archs.Psx.Streamers;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Psx.Converters;

[Service]
public sealed class PsxBeatmaniaDecoder(IBmDataStreamReader bmDataStreamReader)
    : IPsxBeatmaniaDecoder
{
    public List<BmDataFile> DecodeBmData(Stream source, long length)
    {
        using var memOwner = MemoryPool<byte>.Shared.Rent((int)length);
        var mem = memOwner.Memory[..(int)length];
        source.ReadExactly(mem.Span);
        using var pak = new ReadOnlyMemoryStream(mem);

        Span<byte> temp = stackalloc byte[16];
        var files = new List<BmDataFile>();

        for (var i = 0; i < pak.Length - 0x7FF; i += 0x800)
        {
            pak.Position = i;

            pak.ReadExactly(temp);

            if (ReadInt32LittleEndian(temp) == ReadInt32LittleEndian(temp[8..]) &&
                ReadInt32LittleEndian(temp) is > 0 and < 32768 &&
                ReadInt32LittleEndian(temp[4..]) is > 0 and < 1024 &&
                ReadInt32LittleEndian(temp[8..]) is > 0 and < 32768 &&
                ReadInt32LittleEndian(temp[12..]) > 0)
            {
                try
                {
                    pak.Position = i;
                    var bmDataPakDirectory = bmDataStreamReader.ReadDirectory(pak);
                    var bmFiles = bmDataStreamReader.ReadEntries(pak, bmDataPakDirectory);
                    files.AddRange(bmFiles.Select((x, i) => CreateFile(x, i) with
                    {
                        Group = i
                    }));
                }
                catch
                {
                    // No action taken.
                }
            }
        }

        return files;
    }

    private static BmDataFile CreateFile(ReadOnlyMemory<byte> data, int index)
    {
        var span = data.Span;
        var type = BmDataPakEntryType.Unknown;

        if (span.Length >= 4 &&
            (span.Length & 3) == 0 &&
            ReadInt32LittleEndian(span[^4..]) == 0x00007FFF)
        {
            type = BmDataPakEntryType.Chart;
        }
        else if (span.Length >= 16 &&
                 (span.Length & 7) == 0 &&
                 ReadInt32BigEndian(span) < 0x1000 &&
                 ReadInt32BigEndian(span) >= 0x0800 &&
                 ReadInt32BigEndian(span) != 0 &&
                 (ReadInt32BigEndian(span[4..]) & 7) == 0 &&
                 ReadInt32BigEndian(span[4..]) != 0 &&
                 span[8..16].IndexOfAnyExcept((byte)0x00) < 0)
        {
            type = BmDataPakEntryType.Keysound;
        }

        return new BmDataFile
        {
            Index = index,
            Data = data,
            Type = type
        };
    }
}