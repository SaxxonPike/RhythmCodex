using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.FileSystems.Cd.Model;

namespace RhythmCodex.FileSystems.Cd.Streamers;

public sealed class StreamCdSectorCollection(Stream stream, long? length = null)
    : ICdSectorCollection
{
    private class StreamCdSector(int number, Stream stream) : ICdSector
    {
        public int Number { get; } = number;

        public ReadOnlyMemory<byte> Data
        {
            get
            {
                stream.Position = (long)Number * 2352;
                var data = new byte[2352];
                stream.ReadExactly(data.AsSpan());
                return data;
            }
        }
    }

    public IEnumerator<ICdSector> GetEnumerator() =>
        Enumerable.Range(0, Count)
            .Select(i => new StreamCdSector(i, stream))
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public int Count =>
        (int)(Length / 2352);

    public ICdSector this[int index] =>
        new StreamCdSector(index, stream);

    public long Length => length ?? stream.Length;
}