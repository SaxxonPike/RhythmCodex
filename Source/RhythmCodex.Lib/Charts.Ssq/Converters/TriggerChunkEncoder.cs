using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;

namespace RhythmCodex.Charts.Ssq.Converters;

[Service]
public class TriggerChunkEncoder : ITriggerChunkEncoder
{
    public Memory<byte> Convert(IEnumerable<Trigger> triggers)
    {
        var triggerList = triggers.AsCollection();

        using var mem = new MemoryStream();
        using var writer = new BinaryWriter(mem);

        writer.Write(triggerList.Count);

        foreach (var trigger in triggerList)
            writer.Write(trigger.MetricOffset);

        foreach (var trigger in triggerList)
            writer.Write(trigger.Id);

        return mem.ToArray();
    }
}