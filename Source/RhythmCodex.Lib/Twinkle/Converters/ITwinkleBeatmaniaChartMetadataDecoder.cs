using RhythmCodex.Charting.Models;

namespace RhythmCodex.Twinkle.Converters;

public interface ITwinkleBeatmaniaChartMetadataDecoder
{
    void AddMetadata(Chart chart, int index);
}