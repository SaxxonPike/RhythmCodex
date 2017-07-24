using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public class TriggerChunkDecoder : ITriggerChunkDecoder
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
    }
}
