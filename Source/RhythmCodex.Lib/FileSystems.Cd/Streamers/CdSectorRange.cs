using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.FileSystems.Cd.Model;

namespace RhythmCodex.FileSystems.Cd.Streamers;

public class CdSectorRange(ICdSectorCollection sectors, int start, int count) : ICdSectorCollection
{
    public IEnumerator<ICdSector> GetEnumerator() =>
        Enumerable
            .Range(start, count)
            .Select(i => sectors[i])
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public int Count => count;

    public ICdSector this[int index] =>
        sectors[index + start];

    public long Length => count * (long)CdSector.RawSectorSize;

    public void Dispose()
    {
    }
}