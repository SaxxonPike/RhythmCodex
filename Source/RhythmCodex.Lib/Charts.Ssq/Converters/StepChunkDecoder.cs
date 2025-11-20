using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Charts.Ssq.Converters;

[Service]
public class StepChunkDecoder : IStepChunkDecoder
{
    public List<Step> Convert(ReadOnlyMemory<byte> data)
    {
        var reader = new MemoryReader(data);
        var count = reader.ReadInt32();

        var metricOffsets = Enumerable
            .Range(0, count)
            .Select(_ => reader.ReadInt32())
            .ToList();

        var panels = reader.ReadBytes(count);
        var padding = count & 1;
        reader.ReadBytes(padding);

        return Enumerable
            .Range(0, count)
            .Select(i => new Step
            {
                MetricOffset = metricOffsets[i],
                Panels = panels[i],
                // ReSharper disable once AccessToDisposedClosure
                ExtraPanels = panels[i] == 0
                    ? reader.ReadByte()
                    : null,
                // ReSharper disable once AccessToDisposedClosure
                ExtraPanelInfo = panels[i] == 0
                    ? reader.ReadByte()
                    : null
            })
            .ToList();
    }
}