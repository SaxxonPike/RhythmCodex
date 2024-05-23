using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Extensions;

public static class ChartExtensions
{
    private static BigRational GetLinearRate(BigRational bpm) => new(bpm.Denominator * 240, bpm.Numerator);

    /// <summary>
    /// Running this will normalize all metric offsets so each measure line lands on an integer.
    /// Measure lengths are populated on measure lines.
    /// </summary>
    private static void NormalizeMetricOffsets(IChart chart)
    {
        if (chart.Events.Any(ev => ev[NumericData.MetricOffset] == null))
            throw new RhythmCodexException($"All events must have a {nameof(NumericData.MetricOffset)}.");

        if (!chart.Events.Any() ||
            !chart.Events.Any(ev => ev[FlagData.Measure] == true || ev[FlagData.End] == true))
            return;
            
        // Add leading measure to make calculations work if needed
        if (!chart.Events.Any(ev => ev[NumericData.MetricOffset] == 0 && (ev[FlagData.Measure] == true || ev[FlagData.End] == true)))
        {
            chart.Events.Add(new Event
            {
                [FlagData.Measure] = true,
                [NumericData.MetricOffset] = BigRational.Zero
            });
        }

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
            .MaxBy(o => o);
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
                ev[NumericData.MetricOffset] = (ev[NumericData.MetricOffset] - baseOffset) / measureLength + i;
                eligibleEvents.Remove(ev);
            }
        }
    }

    public static void QuantizeMetricOffsets(this IChart chart, BigRational quantization)
    {
        if (chart.Events.Any(ev => ev[NumericData.MetricOffset] == null))
            throw new RhythmCodexException($"All events must have a {nameof(NumericData.MetricOffset)}.");

        foreach (var ev in chart.Events)
        {
            var temp = ev[NumericData.MetricOffset];
            temp *= quantization;
            temp = temp.Value.GetWholePart();
            temp /= quantization;
            ev[NumericData.MetricOffset] = temp;
        }
    }

    public static void PopulateMetricOffsets(this IChart chart, BigRational? referenceLinear = null, BigRational? referenceMetric = null)
    {
        if (chart.Events.Any(ev => ev[NumericData.LinearOffset] == null))
            throw new RhythmCodexException($"All events must have a {nameof(NumericData.LinearOffset)}.");

        var orderedEvents = chart.Events.OrderBy(e => e[NumericData.LinearOffset]).AsList();
            
        var bpm = chart[NumericData.Bpm] ??
                  chart.Events.FirstOrDefault(
                      ev => ev[NumericData.Bpm] != null &&
                            ev[NumericData.Bpm] > BigRational.Zero)?[NumericData.Bpm];

        if (bpm == null)
            throw new RhythmCodexException(
                $"Either the chart or an event must specify {nameof(NumericData.Bpm)}.");

        referenceMetric ??= BigRational.Zero;
        referenceLinear ??= BigRational.Zero;
        var linearRate = GetLinearRate(bpm.Value);

        foreach (var ev in orderedEvents)
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

    public static void PopulateLinearOffsets(this IChart chart, BigRational? referenceLinear = null, BigRational? referenceMetric = null)
    {
        if (chart.Events.Any(ev => ev[NumericData.MetricOffset] == null))
            throw new RhythmCodexException($"All events must have a {nameof(NumericData.MetricOffset)}.");

        var orderedEvents = chart.Events.OrderBy(e => e[NumericData.MetricOffset]).AsList();
            
        var bpm = chart[NumericData.Bpm] ??
                  orderedEvents.FirstOrDefault(
                      ev => ev[NumericData.Bpm] != null &&
                            ev[NumericData.Bpm] > BigRational.Zero)?[NumericData.Bpm];

        if (bpm == null)
            throw new RhythmCodexException(
                $"Either the chart or an event must specify {nameof(NumericData.Bpm)}.");

        referenceMetric = referenceMetric ?? BigRational.Zero;
        referenceLinear = referenceLinear ?? BigRational.Zero;
        var linearRate = GetLinearRate(bpm.Value);
        var pendingStop = BigRational.Zero;

        foreach (var ev in orderedEvents)
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

    public static BigRational GetZeroLinearReference(this IChart chart, BigRational? referenceLinear = null,
        BigRational? referenceMetric = null)
    {
        if (chart.Events.Any(ev => ev[NumericData.MetricOffset] == null))
            throw new RhythmCodexException($"All events must have a {nameof(NumericData.MetricOffset)}.");

        var orderedEvents = chart.Events.OrderBy(e => e[NumericData.MetricOffset]).AsList();
            
        var bpm = chart[NumericData.Bpm] ??
                  orderedEvents.FirstOrDefault(
                      ev => ev[NumericData.Bpm] != null &&
                            ev[NumericData.Bpm] > BigRational.Zero)?[NumericData.Bpm];

        if (bpm == null)
            throw new RhythmCodexException(
                $"Either the chart or an event must specify {nameof(NumericData.Bpm)}.");
            
        referenceMetric = referenceMetric ?? BigRational.Zero;
        referenceLinear = referenceLinear ?? BigRational.Zero;
        var linearRate = GetLinearRate(bpm.Value);
        return ((BigRational.Zero - referenceMetric) * linearRate + referenceLinear).Value;
    }
}