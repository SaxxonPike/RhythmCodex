using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Cd.Streamers;

/// <summary>
/// Represents a collection of CD sectors that are loaded on demand from a
/// stream. The underlying stream position will be moved as data is needed.
/// </summary>
[NotService]
public sealed class StreamCdSectorCollection(Stream stream, long? length = null)
    : ICdSectorCollection
{
    /// <summary>
    /// Represents a CD sector. The sector is read from the stream on demand.
    /// </summary>
    private sealed class StreamCdSector(int number, Stream stream) : ICdSector
    {
        /// <summary>
        /// Holds the reference to the data. The data is discarded by the GC
        /// as necessary and refreshed on demand.
        /// </summary>
        private WeakReference<byte[]>? _dataRef;

        /// <inheritdoc />
        public int Number { get; } = number;

        /// <inheritdoc />
        public ReadOnlyMemory<byte> Data
        {
            get
            {
                if (_dataRef != null && _dataRef.TryGetTarget(out var data))
                    return data;

                stream.Position = (long)Number * CdSector.RawSectorSize;
                data = new byte[CdSector.RawSectorSize];
                stream.ReadExactly(data.AsSpan());

                if (_dataRef == null)
                    _dataRef = new WeakReference<byte[]>(data);
                else
                    _dataRef.SetTarget(data);

                return data;
            }
        }
    }

    /// <inheritdoc />
    public IEnumerator<ICdSector> GetEnumerator() =>
        Enumerable.Range(0, Count)
            .Select(i => new StreamCdSector(i, stream))
            .GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    /// <inheritdoc />
    public int Count =>
        (int)(Length / CdSector.RawSectorSize);

    /// <inheritdoc />
    public ICdSector this[int index] =>
        new StreamCdSector(index, stream);

    /// <inheritdoc />
    public long Length =>
        length ?? stream.Length;
}