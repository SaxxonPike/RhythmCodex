using System;
using System.IO;
using System.Linq;
using RhythmCodex.Compressions.BemaniLz.Processors;
using RhythmCodex.Extensions;
using RhythmCodex.Games.Ddr.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Games.Ddr.Streamers;

[Service]
public class DdrPs2FileDataTableChunkStreamReader(IBemaniLzDecoder bemaniLzDecoder)
    : IDdrPs2FileDataTableChunkStreamReader
{
    public DdrPs2FileDataTableChunk? GetUnbound(Stream stream)
    {
        var cache = new CachedStream(stream);
        var cacheReader = new BinaryReader(cache);
        using var snapshot = new SnapshotStream(cache);
        var snapshotReader = new BinaryReader(snapshot);
        var firstFile = snapshotReader.ReadInt32();
        var tableSize = firstFile / 4 * 4;
        var table = Enumerable.Range(0, firstFile / 4).Select(_ => snapshotReader.ReadInt32()).ToArray();
        var skip = firstFile - tableSize;
        int index;

        if (skip > 0)
            snapshotReader.ReadBytes(skip);

        for (index = 1; index < table.Length; index++)
            if (table[index] < table[index - 1])
                break;
            
        // Warning: big heckin' hack.
        // We're looking for a TCB table, which puts headers on all the file entries.
        // There's no way to tell easily outside of this function, so...

        var prevPosition = cache.Position;
        cache.Position = table[0];
        var headTest0 = cacheReader.ReadInt32();
        var headTest1 = cacheReader.ReadInt32();
        var headTest2 = cacheReader.ReadInt32();

        var hasHeaders = headTest0 > 0 && headTest1 > 0 && headTest2 > 0 &&
                         headTest2 > headTest1 && headTest1 > headTest0 &&
                         (headTest0 & 0x3) == 0 &&
                         (headTest0 | headTest1 | headTest2) >> 24 == 0;

        cache.Position = prevPosition;
        var desiredPosition = table[index - 1];
        snapshotReader.ReadBytes((int) (desiredPosition - prevPosition));

        if (hasHeaders)
            snapshotReader.ReadBytes(0xC);
            
        try
        {
            bemaniLzDecoder.Decode(snapshot);
        }
        catch (Exception)
        {
            return null;
        }

        return new DdrPs2FileDataTableChunk
        {
            Data = snapshot.ToArray(),
            HasHeaders = hasHeaders
        };
    }

    public DdrPs2FileDataTableChunk GetBound(Stream stream)
    {
        var cache = new CachedStream(stream);
        var cacheReader = new BinaryReader(cache);

        var entryCount = cacheReader.ReadInt32();
        var offsets = Enumerable.Range(0, entryCount).Select(_ => cacheReader.ReadInt32()).ToArray();
        var lengths = Enumerable.Range(0, entryCount).Select(_ => cacheReader.ReadInt32()).ToArray();
        var entries = offsets.Select((e, i) => (Offset: e, Length: lengths[i])).OrderBy(x => x.Offset).ToArray();
        var max = entries.Last().Use(x => x.Length + x.Offset);

        cache.Rewind();
        var snapshot = new SnapshotStream(cache);
        snapshot.TryRead(new byte[max], 0, max);
        return new DdrPs2FileDataTableChunk
        {
            Data = snapshot.ToArray(),
            HasHeaders = false
        };
    }
}