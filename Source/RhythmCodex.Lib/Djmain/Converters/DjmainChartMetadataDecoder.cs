using RhythmCodex.Beatmania.Models;
using RhythmCodex.Charting.Models;
using RhythmCodex.Djmain.Model;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Djmain.Converters
{
    [Service]
    public class DjmainChartMetadataDecoder : IDjmainChartMetadataDecoder
    {
        public void AddMetadata(IChart chart, DjmainChunkFormat format, int index)
        {
            switch (format)
            {
                case DjmainChunkFormat.First:
                case DjmainChunkFormat.Second:
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
}