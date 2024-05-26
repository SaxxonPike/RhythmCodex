using System;
using System.Collections.Generic;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;
using Saxxon.StreamCursors;

namespace RhythmCodex.Ssq.Converters;

[Service]
public class TriggerChunkDecoder : ITriggerChunkDecoder
{
    public List<Trigger> Convert(ReadOnlySpan<byte> data)
    {
        var cursor = data.ReadS32L(out var count);

        var metricOffsets = new int[count];
        for (var i = 0; i < count; i++)
            cursor = cursor.ReadS32L(out metricOffsets[i]);

        var ids = new short[count];
        for (var i = 0; i < count; i++)
            cursor = cursor.ReadS16L(out ids[i]);

        var result = new List<Trigger>();
        for (var i = 0; i < count; i++)
            result.Add(new Trigger
            {
                Id = ids[i],
                MetricOffset = metricOffsets[i]
            });

        return result;
    }
}