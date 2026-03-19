using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Psx.Streamers;

/// <inheritdoc />
[Service]
public sealed class PsxBeatmaniaFileStreamReader : IPsxBeatmaniaFileStreamReader
{
    /// <inheritdoc />
    public List<PsxBeatmaniaFileEntry> ReadDirectory(Stream stream)
    {
        var baseOffset = stream.Position;

        Span<byte> buffer = stackalloc byte[8];

        stream.ReadExactly(buffer);
        var dictCount = ReadInt32LittleEndian(buffer[4..]);

        var dict = new byte[dictCount * 8];
        stream.ReadExactly(dict);

        var result = new List<PsxBeatmaniaFileEntry>();
        for (var i = 0; i < dictCount; i++)
        {
            var entryData = dict.AsSpan(i << 3, 8);
            var offset = ReadInt32LittleEndian(entryData) * 0x800;

            result.Add(new PsxBeatmaniaFileEntry
            {
                Index = i,
                Length = ReadInt32LittleEndian(entryData[4..]),
                Offset = offset,
                BaseOffset = baseOffset
            });
        }

        return result;
    }

    /// <inheritdoc />
    public IEnumerable<ReadOnlyMemory<byte>> ReadEntries(Stream stream, IEnumerable<PsxBeatmaniaFileEntry> entries)
    {
        foreach (var entry in entries)
        {
            stream.Seek(entry.BaseOffset + entry.Offset, SeekOrigin.Begin);
            var data = new byte[entry.Length];
            stream.ReadExactly(data);
            yield return data;
        }
    }
}