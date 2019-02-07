using RhythmCodex.Charting;
using RhythmCodex.Charting.Models;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Infrastructure;
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
                        chart[NumericData.Difficulty] = 3;
                    break;
                }
                default:
                {
                    switch (index)
                    {
                        case 0:
                        case 3:
                            chart[NumericData.Difficulty] = 3;
                            break;
                        case 1:
                        case 4:
                            chart[NumericData.Difficulty] = 2;
                            break;
                        case 2:
                        case 5:
                            chart[NumericData.Difficulty] = 4;
                            break;
                    }

                    break;
                }
            }
        }
    }
}