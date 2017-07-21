using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Converters;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public class TriggerConverter : IConverter<byte[], List<Trigger>>, IConverter<IEnumerable<Trigger>, byte[]>
    {
        public List<Trigger> Convert(byte[] data)
        {
            using (var mem = new MemoryStream(data))
            using (var reader = new BinaryReader(mem))
            {
                var count = reader.ReadInt32();

                return Enumerable
                    .Range(0, count)
                    .Select(i => new Trigger
                    {
                        Type = reader.ReadByte(),
                        Parameter = reader.ReadByte()
                    })
                    .ToList();
            }
        }

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
