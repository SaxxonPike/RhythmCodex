using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[Service]
public class StepChunkEncoder : IStepChunkEncoder
{
    public Memory<byte> Convert(IReadOnlyCollection<Step> steps)
    {
        var count = steps.Count;

        using var mem = new MemoryStream();
        using var writer = new BinaryWriter(mem);
        writer.Write(count);

        foreach (var offset in steps.Select(s => s.MetricOffset))
            writer.Write(offset);

        foreach (var panel in steps.Select(s => s.Panels))
            writer.Write(panel);

        if ((count & 1) != 0)
            writer.Write((byte) 0);

        foreach (var extraPanel in steps.Where(s => s.ExtraPanels != null || s.ExtraPanelInfo != null))
        {
            writer.Write(extraPanel.ExtraPanels ?? 0);
            writer.Write(extraPanel.ExtraPanelInfo ?? 0);
        }

        return mem.ToArray();
    }
}