using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Cd.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Cd.Streamers;

[NotService]
public class CdSectorOnDiskCollection(int total, Func<int, Memory<byte>> read) : IReadOnlyList<ICdSector>
{
    private class OnDiskCdSector(int number, Func<int, Memory<byte>> read) : ICdSector
    {
        public int Number { get; } = number;
        public Memory<byte> Data => read(Number);
    }

    public IEnumerator<ICdSector> GetEnumerator() => 
        Enumerable.Range(0, total).Select(i => new OnDiskCdSector(i, read)).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int Count => total;
    public ICdSector this[int index] => new OnDiskCdSector(index, read);
}