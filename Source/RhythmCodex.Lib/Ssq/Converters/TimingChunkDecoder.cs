using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[Service]
public class TimingChunkDecoder : ITimingChunkDecoder
{
    public List<Timing> Convert(ReadOnlyMemory<byte> data)
    {
        using var mem = new ReadOnlyMemoryStream(data);
        using var reader = new BinaryReader(mem);
        var count = reader.ReadInt32();

        var metricOffsets = Enumerable
            .Range(0, count)
            .Select(_ => reader.ReadInt32())
            .ToArray();

        var linearOffsets = Enumerable
            .Range(0, count)
            .Select(_ => reader.ReadInt32())
            .ToArray();

        return Enumerable
            .Range(0, count)
            .Select(i => new Timing
            {
                LinearOffset = linearOffsets[i],
                MetricOffset = metricOffsets[i]
            })
            .ToList();
    }
}