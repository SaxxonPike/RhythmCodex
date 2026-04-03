using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.FileSystems.Cue.Model;

namespace RhythmCodex.FileSystems.Cue.Processors;

public class CueCdSectorCollection(CueFile cue, Func<string, Stream> openFile) : ICdSectorCollection, IDisposable
{
    private List<(int Start, string FileName, int BytesPerSector, string? Type)> _trackRanges = [];
    private readonly Dictionary<string, Stream> _streams = [];

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
            track.Type
        )).ToList();
    }

    private (Stream? Stream, int BytesPerSector, int Start) GetStreamForSector(int sectorNumber)
    {
        EnsureCueCache();

        var track = _trackRanges.FirstOrDefault(x => x.Start <= sectorNumber);

        if (track.FileName == null!)
            return default;

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

        //
        // Position the stream accordingly and read the sector data.
        //

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

    public void Dispose()
    {
        foreach (var stream in _streams.Values)
            stream.Dispose();
    }

    public IEnumerator<ICdSector> GetEnumerator() =>
        Enumerable
            .Range(0, 360000)
            .Select(GetSector)
            .TakeWhile(x => x != null)
            .GetEnumerator()!;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => 360000;

    public ICdSector this[int index] =>
        GetSector(index)!;

    public long Length => Count * (long)CdSector.RawSectorSize;
}