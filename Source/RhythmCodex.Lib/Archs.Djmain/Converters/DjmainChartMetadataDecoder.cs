using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Beatmania.Models;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Archs.Djmain.Converters;

[Service]
public class DjmainChartMetadataDecoder : IDjmainChartMetadataDecoder
{
    public void AddMetadata(Chart chart, DjmainChunkFormat format, int index)
    {
        switch (format)
        {
            case DjmainChunkFormat.BeatmaniaFirst:
            case DjmainChunkFormat.BeatmaniaSecond:
            {
                if (index == 0)
                    chart[NumericData.Difficulty] = BeatmaniaDifficultyConstants.NormalId;
                break;
            }
            default:
            {
                switch (index)
                {
                    case 0:
                    case 3:
                        chart[NumericData.Difficulty] = BeatmaniaDifficultyConstants.NormalId;
                        break;
                    case 1:
                    case 4:
                        chart[NumericData.Difficulty] = BeatmaniaDifficultyConstants.LightId;
                        break;
                    case 2:
                    case 5:
                        chart[NumericData.Difficulty] = BeatmaniaDifficultyConstants.AnotherId;
                        break;
                }

                break;
            }
        }
    }
}