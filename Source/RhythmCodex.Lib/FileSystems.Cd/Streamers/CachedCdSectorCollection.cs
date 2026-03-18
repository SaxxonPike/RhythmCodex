using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.FileSystems.Cd.Model;

namespace RhythmCodex.FileSystems.Cd.Streamers;

/// <summary>
/// Wraps a cache around a collection of CD sectors.
/// </summary>
public class CachedCdSectorCollection(ICdSectorCollection sectors)
    : ICdSectorCollection
{
    private readonly Dictionary<int, WeakReference<ICdSector>> _cache = [];

    public IEnumerator<ICdSector> GetEnumerator() =>
        Enumerable
            .Range(0, sectors.Count)
            .Select(GetOrCacheSector)
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public int Count => sectors.Count;

    public ICdSector this[int index] =>
        GetOrCacheSector(index);

    private ICdSector GetOrCacheSector(int index)
    {
        ICdSector? sector;

        if (!_cache.TryGetValue(index, out var weakRef))
        {
            sector = sectors[index];
            weakRef = new WeakReference<ICdSector>(sector);
            _cache[index] = weakRef;
        }
        else if (!weakRef.TryGetTarget(out sector))
        {
            sector = sectors[index];
            weakRef.SetTarget(sector);
        }

        return sector;
    }

    public long Length => sectors.Length;
}