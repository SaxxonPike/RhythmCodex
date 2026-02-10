using System;
using System.Collections.Generic;
using RhythmCodex.Archs.Firebeat.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Firebeat.Converters;

[Service]
public class FirebeatSampleDecoder : IFirebeatSampleDecoder
{
    public Dictionary<int, FirebeatSample> Decode(
        ReadOnlySpan<byte> data,
        IEnumerable<KeyValuePair<int, FirebeatSampleInfo>> infos
    )
    {
        var samples = new Dictionary<int, FirebeatSample>();

        foreach (var (sampleId, sampleInfo) in infos)
        {
            if (ExtractSample(data, sampleInfo) is { } decodedSample)
                samples[sampleId] = decodedSample;
        }

        return samples;
    }

    private static FirebeatSample? ExtractSample(ReadOnlySpan<byte> data, FirebeatSampleInfo info)
    {
        var sampleOffset = unchecked(info.SampleOffset * 2);
        var sampleLength = unchecked(info.SampleLength * 2);

        if (sampleOffset < 0 ||
            sampleOffset >= data.Length ||
            sampleLength < 0 ||
            sampleLength + sampleOffset > data.Length)
            return null;

        return new FirebeatSample
        {
            Info = info,
            Data = data.Slice(sampleOffset, sampleLength).ToArray()
        };
    }
}