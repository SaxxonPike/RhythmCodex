using RhythmCodex.Charting.Models;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters
{
    public interface IDjmainEventMetadataDecoder
    {
        void AddBeatmaniaMetadata(IEvent ev, IDjmainChartEvent ce);
        void AddPopnMetadata(IEvent ev, IDjmainChartEvent ce);
    }
}