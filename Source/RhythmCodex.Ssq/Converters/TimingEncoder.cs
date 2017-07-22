using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Converters;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public class TimingEncoder : IConverter<IEnumerable<Timing>, byte[]>
    {
        public byte[] Convert(IEnumerable<Timing> timings)
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
