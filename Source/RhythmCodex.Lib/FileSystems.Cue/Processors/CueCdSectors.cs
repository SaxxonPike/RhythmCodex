using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.FileSystems.Cue.Model;

namespace RhythmCodex.FileSystems.Cue.Processors;

public class CueCdSectors(CueFile cue, Func<string, Stream> openFile) : IEnumerable<ICdSector>, IDisposable
{
    private List<(int Start, string FileName, int BytesPerSector, string Type)> _trackRanges = [];
    private readonly Dictionary<string, Stream> _streams = [];
    private readonly Dictionary<int, WeakReference<ICdSector>> _sectorCache = [];
    private readonly Mutex _mutex = new();

    private void EnsureCueCache()
    {
        if (_trackRanges.Count != 0)
            return;

        //
        // Track ranges are stored in reverse order to make short-circuiting
        // out of the track-to-stream forward-only lookup possible.
        //

        _trackRanges = cue.Tracks.OrderByDescending(t => t.Indices[1]).Select(track => (
            Start: track.Indices[1] + track.Pregap,
            FileName: track.FileName!,
            BytesPerSector: track.StoredBytesPerSector,
            Type: track.Type!
        )).ToList();
    }

    private (Stream? Stream, int BytesPerSector, int Start) GetStreamForSector(int sectorNumber)
    {
        EnsureCueCache();

        var track = _trackRanges.FirstOrDefault(x => x.Start <= sectorNumber);

        if (track.FileName == null!)
            return (null, 0, 0);

        if (_streams.TryGetValue(track.FileName, out var stream))
            return (stream, track.BytesPerSector, sectorNumber - track.Start);

        stream = openFile(track.FileName);
        _streams.Add(track.FileName, stream);
        return (stream, track.BytesPerSector, sectorNumber - track.Start);
    }

    private CueCdSector? GetSector(int sectorNumber)
    {
        var (stream, size, start) = GetStreamForSector(sectorNumber);

        if (stream == null || start * size > stream.Length - size)
            return null;

        stream.Position = start * size;
        var buffer = new byte[size];

        var sector = new CueCdSector
        {
            Number = sectorNumber,
            Data = buffer
        };

        stream.ReadExactly(buffer.AsSpan());
        return sector;
    }

    private ICdSector? GetOrCacheSector(int sectorNumber)
    {
        _mutex.WaitOne();

        ICdSector? sector;
        var weakRef = _sectorCache.GetValueOrDefault(sectorNumber);

        //
        // Not yet referenced.
        //

        if (weakRef == null)
        {
            sector = GetSector(sectorNumber);

            if (sector == null)
            {
                _mutex.ReleaseMutex();
                return null;
            }

            weakRef = new WeakReference<ICdSector>(sector);
            _sectorCache[sectorNumber] = weakRef;
            _mutex.ReleaseMutex();
            return sector;
        }

        //
        // Referenced and cached.
        //

        if (weakRef.TryGetTarget(out sector))
        {
            _mutex.ReleaseMutex();
            return sector;
        }

        //
        // Referenced but not cached.
        //

        sector = GetSector(sectorNumber);
        weakRef.SetTarget(sector!);
        _mutex.ReleaseMutex();
        return sector;
    }

    public void Dispose()
    {
        foreach (var stream in _streams.Values)
            stream.Dispose();
    }

    private sealed class Enumerator(CueCdSectors owner) : IEnumerator<ICdSector>
    {
        private ICdSector? _current;
        private int _sectorNumber = -1;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_sectorNumber >= 0 && _current == null)
                return false;

            _sectorNumber++;
            _current = owner.GetOrCacheSector(_sectorNumber);
            return _current != null;
        }

        public void Reset()
        {
            _current = null;
            _sectorNumber = -1;
        }

        ICdSector IEnumerator<ICdSector>.Current => _current!;

        object IEnumerator.Current => _current!;
    }

    public IEnumerator<ICdSector> GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}