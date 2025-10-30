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
    public class TimingEventEncoder : ITimingEventEncoder
    {
        public TimingChunk Encode(IEnumerable<Event> events, int linearRate, BigRational metricLength,
            BigRational? offset, BigRational? startBpm)
        {
            var result = new List<Timing>();

            var tempoEvents = events
                .Where(ev => ev[NumericData.Bpm] != null || ev[NumericData.Stop] != null)
                .OrderBy(ev => ev[NumericData.MetricOffset])
                .ToList();

            // bit of a hack to generate the last event
            tempoEvents.Add(new Event
            {
                [NumericData.MetricOffset] = metricLength,
                [FlagData.End] = true
            });

            var currentBpm = startBpm ?? tempoEvents.Select(ev => ev[NumericData.Bpm]).First(ev => ev != null);
            var linearReference = offset;
            var metricReference = BigRational.Zero;
            var linearBase = 0;

            // bit of a hack to generate the first event
            result.Add(new Timing
            {
                LinearOffset = (int) (linearReference * linearRate),
                MetricOffset = (int) metricReference
            });

            foreach (var ev in tempoEvents)
            {
                var metricOffset = ev[NumericData.MetricOffset].Value;
                
                if (ev[NumericData.MetricOffset] == metricReference && ev[NumericData.Bpm] == currentBpm)
                    continue;

                var metricDiff = metricOffset - metricReference;
                var linearDiff = 4 / (currentBpm / 60) * metricDiff - linearReference;

                linearBase += (int) (linearDiff * linearRate);
                var metricBase = (int) (metricOffset * SsqConstants.MeasureLength);
                currentBpm = ev[NumericData.Bpm] ?? currentBpm;
                metricReference += metricDiff;
                linearReference += linearDiff;

                result.Add(new Timing
                {
                    LinearOffset = linearBase,
                    MetricOffset = metricBase
                });
            }

            return new TimingChunk
            {
                Timings = result,
                Rate = linearRate
            };
        }
    }
}