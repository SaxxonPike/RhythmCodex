using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Streamers
{
    public class CdSectorOnDiskCollection : IReadOnlyList<ICdSector>
    {
        private readonly int _total;
        private readonly Func<int, byte[]> _read;

        private class OnDiskCdSector : ICdSector
        {
            private readonly Func<int, byte[]> _read;
            private readonly Lazy<byte[]> _data;

            public OnDiskCdSector(int number, Func<int, byte[]> read)
            {
                _read = read;
                Number = number;
                _data = new Lazy<byte[]>(DoRead);
            }

            private byte[] DoRead() => _read(Number);
            public int Number { get; }
            public byte[] Data => _data.Value;
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