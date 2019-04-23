using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class TriggerChunkEncoder : ITriggerChunkEncoder
    {
        public byte[] Convert(IEnumerable<Trigger> triggers)
        {
            var triggerList = triggers.AsList();
            var count = triggerList.Count;

            using (var mem = new MemoryStream())
            using (var writer = new BinaryWriter(mem))
            {
                writer.Write(count);

                foreach (var trigger in triggerList)
                    writer.Write(trigger.MetricOffset);

                foreach (var trigger in triggerList)
                    writer.Write(trigger.Id);

                return mem.ToArray();
            }
        }
    }
}