using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Charts.Models;

namespace RhythmCodex.Archs.Djmain.Converters;

public interface IDjmainChartMetadataDecoder
{
    void AddMetadata(Chart chart, DjmainChunkFormat format, int index);
}