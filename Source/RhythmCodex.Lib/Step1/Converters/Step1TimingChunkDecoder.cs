using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;
using Saxxon.StreamCursors;

namespace RhythmCodex.Step1.Converters;

[Service]
public class Step1TimingChunkDecoder : IStep1TimingChunkDecoder
{
    public TimingChunk Convert(ReadOnlySpan<byte> data)
    {
        return new TimingChunk
        {
            Timings = ConvertInternal(data).ToList(), 
            Rate = 75
        };
    }

    private static List<Timing> ConvertInternal(ReadOnlySpan<byte> data)
    {
        var cursor = data;
        var result = new List<Timing>();

        while (cursor.Length >= 8)
        {
            cursor = cursor
                .ReadS32L(out var metricOffset)
                .ReadS32L(out var linearOffset);

            result.Add(new Timing
            {
                MetricOffset = metricOffset,
                LinearOffset = linearOffset
            });
        }

        return result;
    }
}