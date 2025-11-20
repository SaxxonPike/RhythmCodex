using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;

namespace RhythmCodex.Charts.Ssq.Converters;

[Service]
public class StepChunkEncoder : IStepChunkEncoder
{
    public Memory<byte> Convert(IEnumerable<Step> steps)
    {
        var stepList = steps.AsCollection();
        var count = stepList.Count;

        using var mem = new MemoryStream();
        using var writer = new BinaryWriter(mem);
        writer.Write(count);

        foreach (var offset in stepList.Select(s => s.MetricOffset))
            writer.Write(offset);

        foreach (var panel in stepList.Select(s => s.Panels))
            writer.Write(panel);

        if ((count & 1) != 0)
            writer.Write((byte) 0);

        foreach (var extraPanel in stepList.Where(s => s.ExtraPanels != null || s.ExtraPanelInfo != null))
        {
            writer.Write(extraPanel.ExtraPanels ?? 0);
            writer.Write(extraPanel.ExtraPanelInfo ?? 0);
        }

        return mem.ToArray();
    }
}