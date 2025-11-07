using System.Collections.Generic;
using RhythmCodex.Archs.Twinkle.Model;
using RhythmCodex.Games.Beatmania.Pc.Models;

namespace RhythmCodex.Archs.Twinkle.Converters;

public interface ITwinkleBeatmaniaChartEventConverter
{
    BeatmaniaPc1Event ConvertToBeatmaniaPc1(TwinkleBeatmaniaChartEvent chartEvent);
    List<BeatmaniaPc1Event> ConvertNoteCountsToBeatmaniaPc1(int[] noteCounts);
}