using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Cd.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Cd.Streamers
{
    [NotService]
    public class CdSectorOnDiskCollection : IReadOnlyList<ICdSector>
    {
        private readonly int _total;
        private readonly Func<int, byte[]> _read;

        private class OnDiskCdSector : ICdSector
        {
            private readonly Func<int, byte[]> _read;

            public OnDiskCdSector(int number, Func<int, byte[]> read)
            {
                _read = read;
                Number = number;
            }

            public int Number { get; }
            public byte[] Data => _read(Number);
        }

        public CdSectorOnDiskCollection(int total, Func<int, byte[]> read)
        {
            _total = total;
            _read = read;
        }
        
        public IEnumerator<ICdSector> GetEnumerator() => 
            Enumerable.Range(0, _total).Select(i => new OnDiskCdSector(i, _read)).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int Count => _total;
        public ICdSector this[int index] => new OnDiskCdSector(index, _read);
    }
}