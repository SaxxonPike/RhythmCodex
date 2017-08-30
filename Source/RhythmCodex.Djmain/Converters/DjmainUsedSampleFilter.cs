using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters
{
    public class DjmainUsedSampleFilter : IDjmainUsedSampleFilter
    {
        public IDictionary<int, DjmainSampleInfo> Filter(IDictionary<int, DjmainSampleInfo> samples,
            IEnumerable<DjmainChartEvent> events)
        {
            return events
                .Where(IsNote)
                .Select(ev => (int)ev.Param1)
                .Distinct()
                .Intersect(samples.Select(s => s.Key))
                .ToDictionary(i => i, i => samples[i]);
        }

        private bool IsNote(DjmainChartEvent ev)
        {
            switch (ev.Param0 & 0xF)
            {
                case 0x1:
                case 0x5:
                    return true;
            }
            return false;
        }
    }
}
