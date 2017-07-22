using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Converters;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public class TriggerEncoder : IConverter<IEnumerable<Trigger>, byte[]>
    {
        public byte[] Convert(IEnumerable<Trigger> triggers)
        {
            var triggerList = triggers.ToList();
            var count = triggerList.Count;

            using (var mem = new MemoryStream())
            using (var writer = new BinaryWriter(mem))
            {
                writer.Write(count);

                foreach (var trigger in triggerList)
                {
                    writer.Write(trigger.Type);
                    writer.Write(trigger.Parameter);
                }

                return mem.ToArray();
            }
        }
    }
}
