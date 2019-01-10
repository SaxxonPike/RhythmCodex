using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Extensions
{
    public static class ChartExtensions
    {
        private static BigRational GetLinearRate(BigRational bpm)
            => new BigRational(bpm.Denominator * 240, bpm.Numerator);

        /// <summary>
        /// Running this will normalize all metric offsets so each measure line lands on an integer.
        /// Measure lengths are populated on measure lines.
        /// </summary>
        private static void NormalizeMetricOffsets(IChart chart)
        {
            if (chart.Events.Any(ev => ev[NumericData.MetricOffset] == null))
                throw new RhythmCodexException($"All events must have a {nameof(NumericData.MetricOffset)}.");

            if (!chart.Events.Any() || !chart.Events.Any(ev => ev[FlagData.Measure] == true || ev[FlagData.End] == true))
                return;
            
            // Find all the measure lines. End-of-song counts as a measure line, even if it's
            // invisible in the original game, because it has to for purposes of BMS export.
            var measures = chart
                .Events
                .Where(ev => ev[FlagData.End] == true || ev[FlagData.Measure] == true)
                .Select(ev => ev[NumericData.MetricOffset])
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            
            // This really shouldn't happen but you never know if there will be events after the
            // last measure or end of the song.
            var latestEvent = chart
                .Events
                .Select(ev => ev[NumericData.MetricOffset])
                .OrderBy(o => o)
                .Last();
            while (latestEvent >= measures.Last())
                measures.Add(measures.Last() + BigRational.One);

            var eligibleEvents = chart.Events.Distinct().ToList();
            
            // Perform the normalization.
            for (var i = 0; i < measures.Count - 1; i++)
            {
                var baseOffset = measures[i];
                var endOffset = measures[i + 1];
                var measureLength = endOffset - baseOffset;
                var measureEvents = eligibleEvents
                    .Where(ev => ev[NumericData.MetricOffset] >= baseOffset && ev[NumericData.MetricOffset] < endOffset)
                    .ToList();
                foreach (var ev in measureEvents)
                {
                    if (ev[FlagData.Measure] == true || ev[FlagData.End] == true)
                        ev[NumericData.MeasureLength] = measureLength;
                    ev[NumericData.MetricOffset] = ((ev[NumericData.MetricOffset] - baseOffset) / measureLength) + i;
                    eligibleEvents.Remove(ev);
                }
            }
        }
        
        public static void PopulateMetricOffsets(this IChart chart)
        {
            if (chart.Events.Any(ev => ev[NumericData.LinearOffset] == null))
                throw new RhythmCodexException($"All events must have a {nameof(NumericData.LinearOffset)}.");

            var bpm = chart[NumericData.Bpm] ??
                      chart.Events.FirstOrDefault(
                          ev => ev[NumericData.Bpm] != null && 
                                ev[NumericData.Bpm] > BigRational.Zero && 
                                ev[NumericData.LinearOffset] == 0)?[NumericData.Bpm];

            if (bpm == null)
                throw new RhythmCodexException(
                    $"Either the chart or a zero-time event must specify {nameof(NumericData.Bpm)}.");

            BigRational? referenceMetric = BigRational.Zero;
            BigRational? referenceLinear = BigRational.Zero;
            var linearRate = GetLinearRate(bpm.Value);

            foreach (var ev in chart.Events.OrderBy(e => e[NumericData.LinearOffset]))
            {
                ev[NumericData.MetricOffset] = (ev[NumericData.LinearOffset] - referenceLinear) / linearRate +
                                               referenceMetric;

                if (ev[NumericData.Stop] is BigRational newStop && newStop > BigRational.Zero)
                {
                    referenceLinear += newStop;
                }
                
                if (ev[NumericData.Bpm] is BigRational newTempo && newTempo > BigRational.Zero)
                {
                    linearRate = GetLinearRate(newTempo);
                    referenceMetric = ev[NumericData.MetricOffset];
                    referenceLinear = ev[NumericData.LinearOffset];
                }
            }

            NormalizeMetricOffsets(chart);
        }

        public static void PopulateLinearOffsets(this IChart chart)
        {
            if (chart.Events.Any(ev => ev[NumericData.MetricOffset] == null))
                throw new RhythmCodexException($"All events must have a {nameof(NumericData.MetricOffset)}.");

            var bpm = chart[NumericData.Bpm] ??
                      chart.Events.FirstOrDefault(
                          ev => ev[NumericData.Bpm] != null && 
                                ev[NumericData.Bpm] > BigRational.Zero && 
                                ev[NumericData.LinearOffset] == 0)?[NumericData.Bpm];

            if (bpm == null)
                throw new RhythmCodexException(
                    $"Either the chart or a zero-time event must specify {nameof(NumericData.Bpm)}.");

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

                if (ev[NumericData.Stop] is BigRational newStop && newStop > BigRational.Zero)
                {
                    pendingStop = newStop;
                    referenceMetric = ev[NumericData.MetricOffset];
                    referenceLinear = ev[NumericData.LinearOffset];
                }

                if (ev[NumericData.Bpm] is BigRational newTempo && newTempo > BigRational.Zero)
                {
                    linearRate = GetLinearRate(newTempo);
                    referenceMetric = ev[NumericData.MetricOffset];
                    referenceLinear = ev[NumericData.LinearOffset];
                }
            }
        }
    }
}