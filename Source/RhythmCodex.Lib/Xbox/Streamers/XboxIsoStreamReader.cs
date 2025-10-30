using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Xbox.Converters;
using RhythmCodex.Xbox.Model;

namespace RhythmCodex.Xbox.Streamers;

[Service]
public class XboxIsoStreamReader(IXboxIsoInfoDecoder xboxIsoInfoDecoder) : IXboxIsoStreamReader
{
    private const string MediaSectorId = "MICROSOFT*XBOX*MEDIA";

    public IEnumerable<XboxIsoFileEntry> Read(Stream stream, long length)
    {
        // read xbox info header
        var reader = new BinaryReader(stream);
        var basePosition = stream.Position;
        stream.Position = basePosition + 0x800 * 0x20;
        var mediaSector = reader.ReadBytes(0x800);
        if (Encodings.Ascii.GetString(mediaSector[..20].ToArray()) != MediaSectorId)
            throw new RhythmCodexException("This doesn't appear to be an Xbox ISO.");
        var mediaInfo = xboxIsoInfoDecoder.Decode(mediaSector);

        // read root directory
        basePosition += 0x800L * mediaInfo.DirectorySectorNumber;
        stream.Position = basePosition;
        return ReadDirectory(reader, "", basePosition).ToList();
    }

    private static IEnumerable<XboxIsoFileEntry> ReadDirectory(BinaryReader reader, string path, long basePosition)
    {
        var binaryTableLeft = reader.ReadInt16() * 4;
        var binaryTableRight = reader.ReadInt16() * 4;
            
        var entry = new XboxIsoFileEntry
        {
            StartSector = reader.ReadInt32(),
            FileSize = reader.ReadInt32(),
            Attributes = (XboxIsoFileAttributes) reader.ReadByte()
        };

        var nameLength = reader.ReadByte();
        entry.FileName = $"{path}{Encodings.Utf8.GetString(reader.ReadBytes(nameLength))}";

        if (!entry.Attributes.HasFlag(XboxIsoFileAttributes.Directory))
        {
            yield return entry;
        }
            
        if (binaryTableLeft > 0)
        {
            reader.BaseStream.Position = basePosition + binaryTableLeft;
            foreach (var e in ReadDirectory(reader, path, basePosition))
                yield return e;
        }
            
        if (binaryTableRight > 0)
        {
            reader.BaseStream.Position = basePosition + binaryTableRight;
            foreach (var e in ReadDirectory(reader, path, basePosition))
                yield return e;
        }

        if (entry.Attributes.HasFlag(XboxIsoFileAttributes.Directory))
        {
            var newBase = entry.StartSector * 0x800L;
            reader.BaseStream.Position = newBase;
            foreach (var e in ReadDirectory(reader, $"{entry.FileName}/", newBase))
                yield return e;
        }
    }

    public Memory<byte> Extract(Stream stream, XboxIsoFileEntry entry)
    {
        stream.Position = entry.StartSector * 0x800L;
        var reader = new BinaryReader(stream);
        return reader.ReadBytes(entry.FileSize);
    }
}