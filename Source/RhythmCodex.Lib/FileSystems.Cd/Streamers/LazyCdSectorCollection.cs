using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.FileSystems.Cd.Model;

namespace RhythmCodex.FileSystems.Cd.Streamers;

public sealed class LazyCdSectorCollection(Func<int, ICdSector> accessor, int count)
    : ICdSectorCollection
{
    public IEnumerator<ICdSector> GetEnumerator() =>
        Enumerable.Range(0, Count)
            .Select(accessor)
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public int Count =>
        count;

    public ICdSector this[int index] =>
        accessor(index);

    public long Length => (long)count * 2352;
}