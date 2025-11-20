using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.IoC;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Charts.Step1.Converters;

[Service]
public class Step1StepChunkDecoder : IStep1StepChunkDecoder
{
    public List<Step> Convert(ReadOnlySpan<byte> data)
    {
        return ConvertInternal(data).ToList();
    }

    private static List<Step> ConvertInternal(ReadOnlySpan<byte> data)
    {
        var cursor = data.Skip(4);
        var result = new List<Step>();

        while (cursor.Length >= 8)
        {
            cursor = cursor
                .ReadS32L(out var metricOffset)
                .ReadS32L(out var rawPanels);

            if (rawPanels is -1 or 0)
                continue;

            result.Add(new Step
            {
                MetricOffset = metricOffset,
                Panels = unchecked((byte)CollapsePanels(rawPanels))
            });
        }

        return result;
    }

    private static int CollapsePanels(int rawPanels)
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