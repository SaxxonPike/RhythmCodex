using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Step1.Converters;

[Service]
public class Step1StepChunkDecoder : IStep1StepChunkDecoder
{
    public IList<Step> Convert(byte[] data)
    {
        return ConvertInternal(data).ToList();
    }

    private IEnumerable<Step> ConvertInternal(byte[] data)
    {
        using var mem = new ReadOnlyMemoryStream(data);
        using var reader = new BinaryReader(mem);
        // skip metadata
        reader.ReadInt32();
                
        while (mem.Position < mem.Length - 7)
        {
            var metricOffset = reader.ReadInt32();
            var rawPanels = reader.ReadInt32();
            if (rawPanels == -1 || rawPanels == 0)
                continue;
                    
            yield return new Step
            {
                MetricOffset = metricOffset,
                Panels = unchecked((byte)CollapsePanels(rawPanels))
            }; 
        }
    }

    private int CollapsePanels(int rawPanels)
    {
        var output = 0;
        var buffer = rawPanels;
        for (var index = 0; index < 8; index++)
        {
            if ((buffer & 0xF) != 0)
                output |= 1 << index;
            buffer >>= 4;
        }

        return output;
    }
}