using System;
using System.Collections.Generic;
using RhythmCodex.Archs.Twinkle.Model;
using RhythmCodex.Games.Beatmania.Pc.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Twinkle.Converters;

[Service]
public class TwinkleBeatmaniaChartEventConverter : ITwinkleBeatmaniaChartEventConverter
{
    public List<BeatmaniaPc1Event> ConvertNoteCountsToBeatmaniaPc1(ReadOnlySpan<int> noteCounts)
    {
        var result = new List<BeatmaniaPc1Event>();
        var countsToConvert = Math.Min(noteCounts.Length, 2);

        for (var i = 0; i < countsToConvert; i++)
        {
            var noteCount = noteCounts[i];

            if (noteCount < 1)
                continue;

            result.Add(new BeatmaniaPc1Event
            {
                LinearOffset = 0,
                Parameter0 = 0x10,
                Parameter1 = (byte)i,
                Value = (short)noteCounts[i]
            });
        }

        return result;
    }

    public BeatmaniaPc1Event ConvertToBeatmaniaPc1(TwinkleBeatmaniaChartEvent chartEvent)
    {
        int offset = chartEvent.Offset;
        var param0 = (byte)(chartEvent.Param & 0xF);
        var param1 = (byte)(chartEvent.Param >> 4);
        short value = chartEvent.Value;

        switch ((BeatmaniaPc1EventType)param0)
        {
            case BeatmaniaPc1EventType.Bpm:
            {
                // The format here is different to IIDX PC.
                value |= unchecked((short)(param1 << 8));
                param1 = 1;
                break;
            }
        }

        return new BeatmaniaPc1Event
        {
            LinearOffset = offset,
            Parameter0 = param0,
            Parameter1 = param1,
            Value = value
        };
    }
}