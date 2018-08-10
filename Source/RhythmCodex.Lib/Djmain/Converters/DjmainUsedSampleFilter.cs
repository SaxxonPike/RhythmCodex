﻿using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Converters
{
    [Service]
    public class DjmainUsedSampleFilter : IDjmainUsedSampleFilter
    {
        public IDictionary<int, DjmainSampleInfo> Filter(IDictionary<int, DjmainSampleInfo> samples,
            IEnumerable<DjmainChartEvent> events)
        {
            return events
                .Where(IsNote)
                .Select(ev => (int) ev.Param1)
                .Distinct()
                .Intersect(samples.Select(s => s.Key))
                .ToDictionary(i => i, i => samples[i]);
        }

        private static bool IsNote(DjmainChartEvent ev)
        {
            switch ((DjmainEventType)(ev.Param0 & 0xF))
            {
                case DjmainEventType.SoundSelect:
                case DjmainEventType.Bgm:
                    return true;
                default:
                    return false;
            }
        }
    }
}