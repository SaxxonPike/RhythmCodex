using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Charts.Models;

namespace RhythmCodex.Archs.Djmain.Converters;

public interface IDjmainEventMetadataDecoder
{
    void AddBeatmaniaMetadata(Event ev, DjmainChartEvent ce, bool swapStereo);
    void AddPopnMetadata(Event ev, DjmainChartEvent ce, bool swapStereo);
}