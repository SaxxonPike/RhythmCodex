using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.IoC;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Archs.Djmain.Streamers;

[Service]
public sealed class DjmainSampleInfoStreamReader : IDjmainSampleInfoStreamReader
{
    public Dictionary<int, DjmainSampleInfo> Read(Stream stream, int maxSize)
    {
        return ReadInternal(stream, maxSize);
    }

    private static Dictionary<int, DjmainSampleInfo> ReadInternal(Stream stream, int maxSize)
    {
        var resultList = new Dictionary<int, DjmainSampleInfo>();
        var maxDefs = maxSize / 11;
        Span<byte> buffer = stackalloc byte[11];

        for (var i = 0; i < maxDefs; i++)
        {
            var bytesRead = stream.Read(buffer);
            if (bytesRead < 11)
                break;

            var result = new DjmainSampleInfo
            {
                Channel = buffer[0],
                Frequency = buffer[1..].AsU16L(),
                ReverbVolume = buffer[3],
                Volume = buffer[4],
                Panning = buffer[5],
                Offset = buffer[6..].AsU16L() | ((uint) buffer[8] << 16),
                SampleType = buffer[9],
                Flags = buffer[10]
            };

            resultList.Add(i, result);
        }

        return resultList;
    }
}