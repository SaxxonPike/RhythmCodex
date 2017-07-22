using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Converters;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public class StepEncoder : IConverter<IEnumerable<Step>, byte[]>
    {
        public byte[] Convert(IEnumerable<Step> steps)
        {
            var stepList = steps.ToList();
            var count = stepList.Count;

            using (var mem = new MemoryStream())
            using (var writer = new BinaryWriter(mem))
            {
                writer.Write(count);
                
                foreach (var offset in stepList.Select(s => s.MetricOffset))
                    writer.Write(offset);
                
                foreach (var panel in stepList.Select(s => s.Panels))
                    writer.Write(panel);
                
                foreach (var extraPanel in stepList.Where(s => s.ExtraPanels != null).Select(s => (byte)s.ExtraPanels))
                    writer.Write(extraPanel);

                return mem.ToArray();
            }
        }
    }
}
