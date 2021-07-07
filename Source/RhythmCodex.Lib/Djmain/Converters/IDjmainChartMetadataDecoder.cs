using RhythmCodex.Charting.Models;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters
{
    public interface IDjmainChartMetadataDecoder
    {
        void AddMetadata(Chart chart, DjmainChunkFormat format, int index);
    }
}