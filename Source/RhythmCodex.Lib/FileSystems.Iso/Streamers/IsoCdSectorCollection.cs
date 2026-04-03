using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.FileSystems.Iso.Helpers;
using RhythmCodex.FileSystems.Iso.Processors;

namespace RhythmCodex.FileSystems.Iso.Streamers;

public sealed class IsoCdSectorCollection(Stream stream, IIsoSectorConverter isoSectorConverter)
    : ICdSectorCollection
{
    private sealed class IsoCdSector(int number, Stream stream, IIsoSectorConverter isoSectorConverter) : ICdSector
    {
        private ReadOnlyMemory<byte> _data;

        public int Number { get; } = number;

        public ReadOnlyMemory<byte> Data => GetData();

        private ReadOnlyMemory<byte> GetData()
        {
            if (!_data.IsEmpty)
                return _data;

            Span<byte> data = stackalloc byte[CdSector.CookedSectorSize];
            stream.Position = (long)Number * CdSector.CookedSectorSize;
            stream.ReadExactly(data);
            _data = isoSectorConverter.ConvertCookedToRawSector(Number, data);
            return _data;
        }
    }

    public IEnumerator<ICdSector> GetEnumerator() =>
        Enumerable.Range(0, Count)
            .Select(i => new IsoCdSector(i, stream, isoSectorConverter))
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public int Count =>
        (int)(stream.Length / CdSector.CookedSectorSize);

    public ICdSector this[int index] =>
        new IsoCdSector(index, stream, isoSectorConverter);

    public long Length => Count * CdSector.RawSectorSize;

    public void Dispose()
    {
    }
}