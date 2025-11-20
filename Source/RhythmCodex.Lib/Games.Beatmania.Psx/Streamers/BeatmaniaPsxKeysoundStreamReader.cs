using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Games.Beatmania.Psx.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Vag.Streamers;

namespace RhythmCodex.Games.Beatmania.Psx.Streamers;

[Service]
public class BeatmaniaPsxKeysoundStreamReader(IVagStreamReader vagStreamReader) : IBeatmaniaPsxKeysoundStreamReader
{
    public List<BeatmaniaPsxKeysound> Read(Stream stream)
    {
        var reader = new BinaryReader(stream);

        reader.ReadInt32S(); // directory offset
        var directoryLength = reader.ReadInt32S();
        reader.ReadInt32S();
        reader.ReadInt32S();

        var keysounds = Enumerable
            .Range(0, directoryLength / 0x10)
            .Select(_ => new BeatmaniaPsxKeysound
            {
                DirectoryEntry = new BeatmaniaPsxKeysoundDirectoryEntry
                {
                    Offset = reader.ReadInt32(),
                    Unknown0 = reader.ReadInt32(),
                    Unknown1 = reader.ReadInt32(),
                    Unknown2 = reader.ReadInt32()
                }
            })
            .ToList();
            
        var dataOffset = reader.ReadInt32S();
        var dataLength = reader.ReadInt32S();
        reader.ReadInt32S();
        reader.ReadInt32S();

        var data = reader.ReadBytes(dataLength);
        using var dataStream = new ReadOnlyMemoryStream(data);
        foreach (var keysound in keysounds)
        {
            dataStream.Position = keysound.DirectoryEntry.Offset - dataOffset;
            keysound.Data = vagStreamReader.Read(dataStream, 1, 16);
        }

        return keysounds;
    }
}