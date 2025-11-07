using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Charts.Ssq.Converters
{
    [Service]
    public class TriggerEventEncoder : ITriggerEventEncoder
    {
        public IList<Trigger> Encode(IEnumerable<Event> events)
        {
            return events
                .Where(ev => ev[NumericData.Trigger] != null)
                .Select(ev => new Trigger
                {
                    MetricOffset = (int) (ev[NumericData.MetricOffset] * SsqConstants.MeasureLength),
                    Id = (short) ev[NumericData.Trigger].Value
                })
                .ToList();
        }
    }
}