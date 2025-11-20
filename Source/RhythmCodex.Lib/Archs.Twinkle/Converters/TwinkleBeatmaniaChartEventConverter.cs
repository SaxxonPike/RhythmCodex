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
        return new BeatmaniaPc1Event
        {
            LinearOffset = chartEvent.Offset,
            Parameter0 = (byte) (chartEvent.Param & 0xF),
            Parameter1 = (byte) (chartEvent.Param >> 4),
            Value = chartEvent.Value
        };
    }
}