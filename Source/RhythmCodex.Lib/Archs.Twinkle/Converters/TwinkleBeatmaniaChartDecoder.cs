using System;
using System.Collections.Generic;
using RhythmCodex.Archs.Twinkle.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Twinkle.Converters;

[Service]
public class TwinkleBeatmaniaChartDecoder : ITwinkleBeatmaniaChartDecoder
{
    public List<TwinkleBeatmaniaChartEvent> Decode(ReadOnlySpan<byte> data)
    {
        var result = new List<TwinkleBeatmaniaChartEvent>(data.Length / 4);
        var noteCountMode = true;

        for (var i = 0; i < data.Length; i += 4)
        {
            var item = data.Slice(i, 4);

            if (noteCountMode)
            {
                if (item[0x03] <= 1)
                    continue;
                noteCountMode = false;
            }

            var offset = Bitter.ToInt16S(item);
            if (offset == 0x7FFF)
                break;

            var value = item[0x02];
            var command = item[0x03];

            result.Add(new TwinkleBeatmaniaChartEvent
            {
                Offset = (ushort)offset,
                Param = command,
                Value = value
            });
        }

        return result;
    }

    public int[] GetNoteCounts(ReadOnlySpan<byte> data)
    {
        var result = new int[2];

        for (var i = 0; i < data.Length; i += 4)
        {
            var item = data.Slice(i, 4);

            if (item[0x03] >= 2)
                return result;

            result[item[0x03]] += item[0x02];
        }

        return result;
    }
}