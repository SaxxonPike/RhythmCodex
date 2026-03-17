using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.FileSystems.Iso.Processors;

namespace RhythmCodex.FileSystems.Iso.Streamers;

public sealed class IsoCdSectorCollection(Stream stream, IIsoSectorExpander isoSectorExpander)
    : ICdSectorCollection
{
    private sealed class IsoCdSector(int number, Stream stream, IIsoSectorExpander isoSectorExpander) : ICdSector
    {
        public int Number { get; } = number;

        public ReadOnlyMemory<byte> Data
        {
            get
            {
                Span<byte> data = stackalloc byte[2048];
                stream.Position = (long)Number * 2048;
                stream.ReadExactly(data);
                return isoSectorExpander.Expand2048To2352(Number / 4500, Number / 75 % 60, Number % 75, 1, data);
            }
        }
    }

    public IEnumerator<ICdSector> GetEnumerator() =>
        Enumerable.Range(0, Count)
            .Select(i => new IsoCdSector(i, stream, isoSectorExpander))
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public int Count =>
        (int)(stream.Length / 2048);

    public ICdSector this[int index] =>
        new IsoCdSector(index, stream, isoSectorExpander);

    public long Length => Count * 2352;
}