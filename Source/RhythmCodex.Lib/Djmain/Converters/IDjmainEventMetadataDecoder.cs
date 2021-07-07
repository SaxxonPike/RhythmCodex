using RhythmCodex.Charting.Models;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters
{
    public interface IDjmainEventMetadataDecoder
    {
        void AddBeatmaniaMetadata(Event ev, IDjmainChartEvent ce);
        void AddPopnMetadata(Event ev, IDjmainChartEvent ce);
    }
}