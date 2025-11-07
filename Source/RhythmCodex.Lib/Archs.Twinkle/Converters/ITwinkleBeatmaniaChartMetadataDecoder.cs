using RhythmCodex.Charts.Models;

namespace RhythmCodex.Archs.Twinkle.Converters;

public interface ITwinkleBeatmaniaChartMetadataDecoder
{
    void AddMetadata(Chart chart, int index);
}