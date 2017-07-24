using System.Collections.Generic;
using System.Linq;
using Numerics;
using RhythmCodex.Charting;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public class TimingEventDecoder : ITimingEventDecoder
    {
        public IEnumerable<IEvent> Decode(IEnumerable<Timing> timings, int ticksPerSecond)
        {
            var orderedTimings = timings
                .OrderBy(t => t.LinearOffset)
                .ThenBy(t => t.MetricOffset)
                .ToArray();

            var previous = orderedTimings.First();

            foreach (var timing in orderedTimings.Skip(1))
            {
                var ev = new Event
                {
                    [NumericData.MetricOffset] = previous.MetricOffset,
                    [NumericData.LinearOffset] = previous.LinearOffset
                };

                BigRational deltaOffset = timing.MetricOffset - previous.MetricOffset;
                BigRational deltaTicks = timing.LinearOffset - previous.LinearOffset;

                if (deltaOffset == 0)
                    ev[NumericData.Stop] = deltaTicks / ticksPerSecond;
                else
                    ev[NumericData.Tempo] = deltaOffset / 4096 / (deltaTicks / ticksPerSecond / 240);

                yield return ev;
            }
        }
    }
}
