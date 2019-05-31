using RhythmCodex.Charting.Models;

namespace RhythmCodex.Twinkle.Converters
{
    public interface ITwinkleBeatmaniaChartMetadataDecoder
    {
        void AddMetadata(IChart chart, int index);
    }
}