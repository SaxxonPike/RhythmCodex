using System.Collections.Generic;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters
{
    public interface ITwinkleBeatmaniaChartEventConverter
    {
        BeatmaniaPc1Event ConvertToBeatmaniaPc1(TwinkleBeatmaniaChartEvent chartEvent);
        IList<BeatmaniaPc1Event> ConvertNoteCountsToBeatmaniaPc1(int[] noteCounts);
    }
}