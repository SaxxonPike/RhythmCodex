using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Converters;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public class StepConverter : IConverter<byte[], List<Step>>, IConverter<IEnumerable<IStep>, byte[]>
    {
        public List<Step> Convert(byte[] data)
        {
            using (var mem = new MemoryStream(data))
            using (var reader = new BinaryReader(mem))
            {
                var count = reader.ReadInt32();

                var metricOffsets = Enumerable
                    .Range(0, count)
                    .Select(i => reader.ReadInt32())
                    .ToArray();

                var panels = reader.ReadBytes(count);

                return Enumerable
                    .Range(0, count)
                    .Select(i => new Step
                    {
                        MetricOffset = metricOffsets[i],
                        Panels = panels[i],
                        ExtraPanels = panels[i] == 0 ? (int?)reader.ReadByte() : null
                    })
                    .ToList();
            }
        }
        
        public byte[] Convert(IEnumerable<IStep> steps)
        {
            var stepList = steps.ToList();
            var count = stepList.Count;

            using (var mem = new MemoryStream())
            using (var writer = new BinaryWriter(mem))
            {
                writer.Write(count);
                
                foreach (var offset in stepList.Select(s => s.MetricOffset))
                    writer.Write(offset);
                
                foreach (var panel in stepList.Select(s => (byte)s.Panels))
                    writer.Write(panel);
                
                foreach (var extraPanel in stepList.Where(s => s.ExtraPanels != null).Select(s => (byte)s.ExtraPanels))
                    writer.Write(extraPanel);

                return mem.ToArray();
            }
        }
    }
}
