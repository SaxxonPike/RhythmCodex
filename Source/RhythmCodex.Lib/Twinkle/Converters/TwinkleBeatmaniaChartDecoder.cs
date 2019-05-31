using System;
using System.Collections.Generic;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Twinkle.Converters
{
    [Service]
    public class TwinkleBeatmaniaChartDecoder : ITwinkleBeatmaniaChartDecoder
    {
        public IList<BeatmaniaPc1Event> Decode(ReadOnlySpan<byte> data, int length)
        {
            var result = new List<BeatmaniaPc1Event>();
            var noteCountMode = true;
            var noteCount = 0;

            for (var i = 0; i < length; i += 4)
            {
                if (noteCountMode)
                {
                    if ((data[i + 0x03] & 0xF) == 0)
                        continue;
                    noteCountMode = false;
                }

                var offset = Bitter.ToInt16S(data, i);
                if (offset == 0x7FFF)
                    break;
                
                var value = data[i + 0x02];
                var command = data[i + 0x03];
                result.Add(new BeatmaniaPc1Event
                {
                    LinearOffset = offset,
                    Parameter0 = (byte)(command & 0xF),
                    Parameter1 = (byte)(command >> 4),
                    Value = value
                });
            }

            return result;
        }
    }
}