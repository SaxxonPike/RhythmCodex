using System;
using System.Collections.Generic;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters;

[Service]
public class TwinkleBeatmaniaChartDecoder : ITwinkleBeatmaniaChartDecoder
{
    public List<TwinkleBeatmaniaChartEvent> Decode(ReadOnlySpan<byte> data, int length)
    {
        var result = new List<TwinkleBeatmaniaChartEvent>();
        var noteCountMode = true;

        for (var i = 0; i < length; i += 4)
        {
            if (noteCountMode)
            {
                if (data[i + 0x03] == 0 || data[i + 0x03] == 1)
                    continue;
                noteCountMode = false;
            }

            var offset = Bitter.ToInt16S(data, i);
            if (offset == 0x7FFF)
                break;
                
            var value = data[i + 0x02];
            var command = data[i + 0x03];
            result.Add(new TwinkleBeatmaniaChartEvent
            {
                Offset = (ushort)offset,
                Param = command,
                Value = value
            });
        }

        return result;
    }

    public int[] GetNoteCounts(ReadOnlySpan<byte> data, int length)
    {
        var result = new int[2];

        for (var i = 0; i < length; i += 4)
        {
            if (data[i + 0x03] != 0 && data[i + 0x03] != 1)
                return result;
            result[data[i + 0x03]] += data[i + 0x02];
        }

        return result;
    }
}