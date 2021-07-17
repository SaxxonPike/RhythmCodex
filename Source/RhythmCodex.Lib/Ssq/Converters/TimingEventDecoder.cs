using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class TimingEventDecoder : ITimingEventDecoder
    {
        public IList<Event> Decode(TimingChunk timingChunk)
        {
            IEnumerable<Event> Do()
            {
                var maxMetric = BigRational.Zero;
                var maxLinear = BigRational.Zero;
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
                    var newMetric = (BigRational) previous.MetricOffset / SsqConstants.MeasureLength;
                    var newLinear = (BigRational) previous.LinearOffset / ticksPerSecond;
                    
                    var ev = new Event
                    {
                        [NumericData.MetricOffset] = newMetric,
                        [NumericData.LinearOffset] = newLinear
                    };

                    BigRational deltaOffset = timing.MetricOffset - previous.MetricOffset;
                    BigRational deltaTicks = timing.LinearOffset - previous.LinearOffset;

                    if (deltaOffset == 0)
                        ev[NumericData.Stop] = deltaTicks / ticksPerSecond;
                    else if (deltaTicks == 0)
                        ev[NumericData.Bpm] = BigRational.PositiveInfinity;
                    else
                        ev[NumericData.Bpm] =
                            deltaOffset / SsqConstants.MeasureLength / (deltaTicks / ticksPerSecond / 240);

                    if (newMetric > maxMetric)
                        maxMetric = newMetric;
                    if (newLinear > maxLinear)
                        maxLinear = newLinear;
                    
                    yield return ev;

                    previous = new
                    {
                        timing.MetricOffset,
                        timing.LinearOffset
                    };
                }

                yield return new Event
                {
                    [NumericData.MetricOffset] = maxMetric,
                    [NumericData.LinearOffset] = maxLinear,
                    [FlagData.End] = true
                };
            }

            return Do().ToList();
        }
    }
}