using RhythmCodex.Charting;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters
{
    public interface IDjmainChartMetadataDecoder
    {
        void AddMetadata(IChart chart, DjmainChunkFormat format, int index);
    }
}