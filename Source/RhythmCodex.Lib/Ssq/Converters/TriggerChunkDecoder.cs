using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[Service]
public class TriggerChunkDecoder : ITriggerChunkDecoder
{
    public List<Trigger> Convert(byte[] data)
    {
        using var mem = new ReadOnlyMemoryStream(data);
        using var reader = new BinaryReader(mem);
        var count = reader.ReadInt32();

        var metricOffsets = Enumerable
            .Range(0, count)
            .Select(_ => reader.ReadInt32())
            .ToArray();

        return Enumerable
            .Range(0, count)
            .Select(i => new Trigger
            {
                // ReSharper disable once AccessToDisposedClosure
                Id = reader.ReadInt16(),
                MetricOffset = metricOffsets[i]
            })
            .ToList();
    }
}