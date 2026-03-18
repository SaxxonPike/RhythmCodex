using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.FileSystems.Cd.Model;

namespace RhythmCodex.FileSystems.Cd.Streamers;

/// <summary>
/// Represents a collection of CD sectors that are loaded on demand via a function.
/// </summary>
public sealed class LazyCdSectorCollection(Func<int, ICdSector> accessor, int count)
    : ICdSectorCollection
{
    /// <inheritdoc />
    public IEnumerator<ICdSector> GetEnumerator() =>
        Enumerable.Range(0, Count)
            .Select(accessor)
            .GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    /// <inheritdoc />
    public int Count =>
        count;

    /// <inheritdoc />
    public ICdSector this[int index] =>
        accessor(index);

    /// <inheritdoc />
    public long Length => (long)count * CdSector.RawSectorSize;
}