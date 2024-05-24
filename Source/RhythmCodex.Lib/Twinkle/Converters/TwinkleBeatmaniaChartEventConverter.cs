using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.IoC;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters;

[Service]
public class TwinkleBeatmaniaChartEventConverter : ITwinkleBeatmaniaChartEventConverter
{
    public List<BeatmaniaPc1Event> ConvertNoteCountsToBeatmaniaPc1(int[] noteCounts)
    {
        IEnumerable<BeatmaniaPc1Event> Do()
        {
            for (var i = 0; i < 2; i++)
            {
                if (noteCounts[i] > 0)
                {
                    yield return new BeatmaniaPc1Event
                    {
                        LinearOffset = 0,
                        Parameter0 = 0x10,
                        Parameter1 = (byte) i,
                        Value = (short) noteCounts[i]
                    };
                }
            }
        }

        return Do().ToList();
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