using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class TimingEventDecoder : ITimingEventDecoder
    {
        public IEnumerable<IEvent> Decode(TimingChunk timingChunk)
        {
            var timings = timingChunk.Timings;
            var ticksPerSecond = timingChunk.Rate;

            var orderedTimings = timings
                .OrderBy(t => t.LinearOffset)
                .ThenBy(t => t.MetricOffset)
                .ToArray();

            var firstOrderedTiming = orderedTimings.First();

            var previous = new
            {
                firstOrderedTiming.MetricOffset,
                firstOrderedTiming.LinearOffset
            };

            foreach (var timing in orderedTimings.Skip(1))
            {
                var ev = new Event
                {
                    [NumericData.MetricOffset] = (BigRational) previous.MetricOffset / SsqConstants.MeasureLength,
                    [NumericData.LinearOffset] = (BigRational) previous.LinearOffset / ticksPerSecond
                };

                BigRational deltaOffset = timing.MetricOffset - previous.MetricOffset;
                BigRational deltaTicks = timing.LinearOffset - previous.LinearOffset;

                if (deltaOffset == 0)
                    ev[NumericData.Stop] = deltaTicks / ticksPerSecond;
                else
                    ev[NumericData.Bpm] =
                        deltaOffset / SsqConstants.MeasureLength / (deltaTicks / ticksPerSecond / 240);

                yield return ev;

                previous = new
                {
                    timing.MetricOffset,
                    timing.LinearOffset
                };
            }
        }
    }
}