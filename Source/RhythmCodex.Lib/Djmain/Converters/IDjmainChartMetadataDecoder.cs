using RhythmCodex.Charting.Models;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters
{
    public interface IDjmainChartMetadataDecoder
    {
        void AddMetadata(IChart chart, DjmainChunkFormat format, int index);
    }
}