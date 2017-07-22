using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Converters;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public class StepDecoder : IConverter<byte[], List<Step>>
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
                        ExtraPanels = panels[i] == 0 ? reader.ReadByte() : (byte?)null
                    })
                    .ToList();
            }
        }
    }
}
