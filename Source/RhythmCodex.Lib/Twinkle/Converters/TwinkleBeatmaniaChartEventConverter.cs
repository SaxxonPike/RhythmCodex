using RhythmCodex.Beatmania.Models;
using RhythmCodex.IoC;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters
{
    [Service]
    public class TwinkleBeatmaniaChartEventConverter : ITwinkleBeatmaniaChartEventConverter
    {
        public BeatmaniaPc1Event ConvertToBeatmaniaPc1(TwinkleBeatmaniaChartEvent chartEvent)
        {
            return new BeatmaniaPc1Event
            {
                LinearOffset = chartEvent.Offset,
                Parameter0 = (byte)(chartEvent.Param & 0xF),
                Parameter1 = (byte)(chartEvent.Param >> 4),
                Value = chartEvent.Value
            };
        }
    }
}