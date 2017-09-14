using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class StepChunkDecoder : IStepChunkDecoder
    {
        public IList<Step> Convert(byte[] data)
        {
            using (var mem = new MemoryStream(data))
            using (var reader = new BinaryReader(mem))
            {
                var count = reader.ReadInt32();

                var metricOffsets = Enumerable
                    .Range(0, count)
                    // ReSharper disable once AccessToDisposedClosure
                    .Select(i => reader.ReadInt32())
                    .AsList();

                var panels = reader.ReadBytes(count);
                var padding = count & 1;
                reader.ReadBytes(padding);
                
                return Enumerable
                    .Range(0, count)
                    .Select(i => new Step
                    {
                        MetricOffset = metricOffsets[i],
                        Panels = panels[i],
                        // ReSharper disable once AccessToDisposedClosure
                        ExtraPanels = panels[i] == 0 
                            ? reader.ReadByte() 
                            : (byte?)null,
                        // ReSharper disable once AccessToDisposedClosure
                        ExtraPanelInfo = panels[i] == 0 
                            ? reader.ReadByte() 
                            : (byte?)null
                    })
                    .AsList();
            }
        }
    }
}
