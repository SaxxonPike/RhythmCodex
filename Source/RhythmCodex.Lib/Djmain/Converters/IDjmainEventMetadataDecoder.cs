using RhythmCodex.Charting.Models;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters;

public interface IDjmainEventMetadataDecoder
{
    void AddBeatmaniaMetadata(Event ev, DjmainChartEvent ce);
    void AddPopnMetadata(Event ev, DjmainChartEvent ce);
}