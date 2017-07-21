using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Converters;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public class TimingConverter : IConverter<byte[], List<Timing>>, IConverter<IEnumerable<ITiming>, byte[]>
    {
        public List<Timing> Convert(byte[] data)
        {
            using (var mem = new MemoryStream(data))
            using (var reader = new BinaryReader(mem))
            {
                var count = reader.ReadInt32();

                var metricOffsets = Enumerable
                    .Range(0, count)
                    .Select(i => reader.ReadInt32())
                    .ToArray();

                var linearOffsets = Enumerable
                    .Range(0, count)
                    .Select(i => reader.ReadInt32())
                    .ToArray();

                return Enumerable
                    .Range(0, count)
                    .Select(i => new Timing
                    {
                        LinearOffset = linearOffsets[i],
                        MetricOffset = metricOffsets[i]
                    })
                    .ToList();
            }
        }

        public byte[] Convert(IEnumerable<ITiming> timings)
        {
            var timingList = timings.ToArray();
            var count = timingList.Length;

            using (var mem = new MemoryStream())
            using (var writer = new BinaryWriter(mem))
            {
                writer.Write(count);
                
                foreach (var offset in timingList.Select(t => t.MetricOffset))
                    writer.Write(offset);

                foreach (var offset in timingList.Select(t => t.LinearOffset))
                    writer.Write(offset);
                
                writer.Flush();

                return mem.ToArray();
            }
        }
    }
}
