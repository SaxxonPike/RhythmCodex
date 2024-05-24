using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[Service]
public class TriggerChunkEncoder : ITriggerChunkEncoder
{
    public Memory<byte> Convert(IReadOnlyCollection<Trigger> triggers)
    {
        using var mem = new MemoryStream();
        using var writer = new BinaryWriter(mem);
        writer.Write(triggers.Count);

        foreach (var trigger in triggers)
            writer.Write(trigger.MetricOffset);

        foreach (var trigger in triggers)
            writer.Write(trigger.Id);

        return mem.ToArray();
    }
}