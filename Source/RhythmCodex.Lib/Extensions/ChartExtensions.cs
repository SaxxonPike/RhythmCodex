using System.Linq;
using Numerics;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Extensions
{
    public static class ChartExtensions
    {
        private static BigRational GetLinearRate(BigRational bpm) 
            => new BigRational(bpm.Denominator * 240, bpm.Numerator);

        public static void PopulateMetricOffsets(this IChart chart)
        {
            if (chart.Events.Any(ev => ev[NumericData.LinearOffset] == null))
                throw new RhythmCodexException($"All events must have a {nameof(NumericData.LinearOffset)}.");

            var bpm = chart[NumericData.Bpm] ??
                chart.Events.FirstOrDefault(ev => ev[NumericData.Bpm] != null && ev[NumericData.LinearOffset] == 0)?[NumericData.Bpm];

            if (bpm == null)
                throw new RhythmCodexException($"Either the chart or a zero-time event must specify {nameof(NumericData.Bpm)}.");
        }

        public static void PopulateLinearOffsets(this IChart chart)
        {
            if (chart.Events.Any(ev => ev[NumericData.MetricOffset] == null))
                throw new RhythmCodexException($"All events must have a {nameof(NumericData.MetricOffset)}.");

            var bpm = chart[NumericData.Bpm] ?? 
                chart.Events.FirstOrDefault(ev => ev[NumericData.Bpm] != null && ev[NumericData.MetricOffset] == 0)?[NumericData.Bpm];
            
            if (bpm == null)
                throw new RhythmCodexException($"Either the chart or a zero-time event must specify {nameof(NumericData.Bpm)}.");

            BigRational? referenceMetric = BigRational.Zero;
            BigRational? referenceLinear = BigRational.Zero;
            var linearRate = GetLinearRate(bpm.Value);
            var pendingStop = BigRational.Zero;
                
            foreach (var ev in chart.Events.OrderBy(e => e[NumericData.MetricOffset]))
            {
                if (pendingStop != BigRational.Zero && ev[NumericData.MetricOffset] > referenceMetric)
                {
                    referenceLinear += pendingStop;
                    pendingStop = BigRational.Zero;
                }

                ev[NumericData.LinearOffset] = (ev[NumericData.MetricOffset] - referenceMetric) * linearRate +
                                               referenceLinear;

                if (ev[NumericData.Stop] is BigRational newStop)
                {
                    pendingStop = newStop;
                    referenceMetric = ev[NumericData.MetricOffset];
                    referenceLinear = ev[NumericData.LinearOffset];
                }
                
                if (ev[NumericData.Bpm] is BigRational newTempo)
                {
                    linearRate = GetLinearRate(newTempo);
                    referenceMetric = ev[NumericData.MetricOffset];
                    referenceLinear = ev[NumericData.LinearOffset];
                }
            }
        }
    }
}
