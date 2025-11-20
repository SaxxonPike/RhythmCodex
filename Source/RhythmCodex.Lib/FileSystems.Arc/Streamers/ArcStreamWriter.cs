using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.FileSystems.Arc.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Arc.Streamers;

/// <inheritdoc />
[Service]
public class ArcStreamWriter : IArcStreamWriter
{
    public void Write(Stream target, IEnumerable<ArcFile> files)
    {
        using var buffer = new MemoryStream();
        var writer = new BinaryWriter(buffer);
        var fileList = files.AsCollection();

        writer.Write(0x19751120);
        writer.Write(0x00000001);
        writer.Write(fileList.Count);
        writer.Write(0x00000002);
        buffer.Position += fileList.Count * 0x10;

        var entries = fileList.Select(file =>
        {
            var entry = new ArcEntry
            {
                CompressedSize = file.CompressedSize,
                DecompressedSize = file.DecompressedSize,
                NameOffset = (int)buffer.Position
            };

            if (file.Name != null)
                writer.Write(Encodings.Cp437.GetBytes(file.Name));

            writer.Write((byte)0x00);
            return (File: file, Entry: entry);
        }).ToList();

        for (var i = 0; i < fileList.Count; i++)
        {
            Pad0X20();
            entries[i].Entry.Offset = (int)buffer.Position;

            if (entries[i].File.Data is var data)
                writer.Write(data.Span);
        }

        Pad0X20();

        buffer.Position = 0x10;

        foreach (var entry in entries)
        {
            writer.Write(entry.Entry.NameOffset);
            writer.Write(entry.Entry.Offset);
            writer.Write(entry.Entry.DecompressedSize);
            writer.Write(entry.Entry.CompressedSize);
        }

        writer.Flush();
        target.Write(buffer.GetBuffer(), 0, (int)buffer.Length);
        return;

        void Pad0X20()
        {
            var paddingLength = (int)(0x20 - (buffer.Position & 0x1F)) & 0x1F;
            if (paddingLength < 1)
                return;
            Span<byte> padding = stackalloc byte[paddingLength];
            if (padding.Length > 0)
                writer.Write(padding);
        }
    }
}