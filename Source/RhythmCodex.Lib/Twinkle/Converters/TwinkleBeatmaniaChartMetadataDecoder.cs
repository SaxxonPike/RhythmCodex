using RhythmCodex.Beatmania.Models;
using RhythmCodex.Charting.Models;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Twinkle.Converters;

[Service]
public class TwinkleBeatmaniaChartMetadataDecoder : ITwinkleBeatmaniaChartMetadataDecoder
{
    public void AddMetadata(Chart chart, int index)
    {
        switch (index)
        {
            case 0:
                chart[StringData.Difficulty] = BeatmaniaDifficultyConstants.FiveKey;
                break;
            case 1:
                chart[NumericData.Difficulty] = BeatmaniaDifficultyConstants.LightId;
                chart[StringData.Difficulty] = BeatmaniaDifficultyConstants.Light;
                break;
            case 2:
                chart[NumericData.Difficulty] = BeatmaniaDifficultyConstants.NormalId;
                chart[StringData.Difficulty] = BeatmaniaDifficultyConstants.Normal;
                break;
            case 3:
                chart[NumericData.Difficulty] = BeatmaniaDifficultyConstants.AnotherId;
                chart[StringData.Difficulty] = BeatmaniaDifficultyConstants.Another;
                break;
        }
    }
}