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
    : ICdSectorCollection, IDisposable
{
    private readonly Dictionary<int, ICdSector> _cache = [];

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
        while (_cache.Count > 256)
        {
            _cache.Remove(_cache.First().Key, out var removed);
            (removed as IDisposable)?.Dispose();
        }

        if (_cache.TryGetValue(index, out var sector)) 
            return sector;

        sector = sectors[index];
        _cache[index] = sector;
        return sector;
    }

    public long Length => sectors.Length;

    public void Dispose() => 
        (sectors as IDisposable)?.Dispose();
}