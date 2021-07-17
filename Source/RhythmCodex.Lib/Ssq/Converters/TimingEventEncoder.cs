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

            // bit of a hack to generate the last event (if we don't have an end)
            if (tempoEvents.All(ev => ev[FlagData.End] != true))
            {
                tempoEvents.Add(new Event
                {
                    [NumericData.MetricOffset] = metricLength,
                    [FlagData.End] = true
                });
            }

            var currentBpm = startBpm ?? tempoEvents.Select(ev => ev[NumericData.Bpm]).First(ev => ev != null).Value;
            var linearReference = BigRational.Zero;
            var metricReference = BigRational.Zero;
            var linearBase = 0;

            // bit of a hack to generate the first event (I didn't come up with this, Konami does this)
            
            result.Add(new Timing
            {
                LinearOffset = (int) (linearReference * linearRate),
                MetricOffset = (int) metricReference
            });

            foreach (var ev in tempoEvents)
            {
                var metricOffset = ev[NumericData.MetricOffset].Value;
                var metricDiff = metricOffset - metricReference;
                var linearDiff = 4 / (currentBpm / 60) * metricDiff;

                void Mark()
                {
                    if (linearDiff <= BigRational.Zero && metricDiff <= BigRational.Zero)
                        return;
                    
                    linearBase += (int) (linearDiff * linearRate);
                    metricReference += metricDiff;
                    linearReference += linearDiff;
                    metricDiff = BigRational.Zero;
                    linearDiff = BigRational.Zero;

                    result.Add(new Timing
                    {
                        LinearOffset = linearBase,
                        MetricOffset = (int) (metricOffset * SsqConstants.MeasureLength)
                    });
                }

                if (ev[NumericData.Bpm] != null && 
                    !(metricDiff <= BigRational.Zero && ev[NumericData.Bpm] == currentBpm))
                {
                    currentBpm = ev[NumericData.Bpm].Value;
                    Mark();
                }

                if (ev[NumericData.Stop] != null)
                {
                    Mark();
                    linearDiff = ev[NumericData.Stop].Value;
                    Mark();
                }

                if (ev[FlagData.End] == true)
                {
                    Mark();
                }
            }

            return new TimingChunk
            {
                Timings = result,
                Rate = linearRate
            };
        }
    }
}