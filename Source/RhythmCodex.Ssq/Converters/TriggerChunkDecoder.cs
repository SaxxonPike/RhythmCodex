using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public class TriggerChunkDecoder : ITriggerChunkDecoder
    {
        public IList<Trigger> Convert(byte[] data)
        {
            using (var mem = new MemoryStream(data))
            using (var reader = new BinaryReader(mem))
            {
                var count = reader.ReadInt32();

                var metricOffsets = Enumerable
                    .Range(0, count)
                    .Select(i => reader.ReadInt32())
                    .ToArray();

                return Enumerable
                    .Range(0, count)
                    .Select(i => new Trigger
                    {
                        Id = reader.ReadInt16(),
                        MetricOffset = metricOffsets[i]
                    })
                    .AsList();
            }
        }
    }
}
