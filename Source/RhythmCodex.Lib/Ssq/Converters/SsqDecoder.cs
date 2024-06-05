using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Ssq.Mappers;
using RhythmCodex.Ssq.Model;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Ssq.Converters;

[Service]
public class SsqDecoder(
    ISsqEventDecoder ssqEventDecoder,
    IPanelMapperSelector panelMapperSelector,
    IChartInfoDecoder chartInfoDecoder,
    ISsqChunkFilter ssqChunkFilter)
    : ISsqDecoder
{
    public List<Chart> Decode(IEnumerable<SsqChunk> data)
    {
        var chunks = data;

        var timings = ssqChunkFilter.GetTimings(chunks);
        var triggers = ssqChunkFilter.GetTriggers(chunks);
        var steps = ssqChunkFilter.GetSteps(chunks);
        var meta = ssqChunkFilter.GetInfos(chunks).FirstOrDefault();

        var charts = steps.Select(sc =>
        {
            var info = chartInfoDecoder.Decode(sc.Id);
            var chart = new Chart
            {
                Events = ssqEventDecoder.Decode(
                    timings,
                    sc.Steps,
                    triggers,
                    panelMapperSelector.Select(sc.Steps, info)),
                [NumericData.Id] = sc.Id,
                [StringData.Difficulty] = info.Difficulty,
                [StringData.Type] = $"{SmGameTypes.Dance}-{info.Type}".ToLowerInvariant()
            };

            var firstTiming = timings.Timings.OrderBy(t => t.LinearOffset).First();
            chart[NumericData.LinearOffset] = chart.GetZeroLinearReference(
                (BigRational)firstTiming.LinearOffset / timings.Rate,
                (BigRational)firstTiming.MetricOffset / SsqConstants.MeasureLength);

            if (meta == null)
                return chart;

            chart[StringData.Title] = meta.Text[0];
            chart[StringData.Subtitle] = meta.Text[1];
            chart[StringData.Artist] = meta.Text[2];

            chart[NumericData.PlayLevel] = sc.Id switch
            {
                0x0113 or 0x0114 or 0x0116 => meta.Difficulties.Span[1],
                0x0213 or 0x0214 or 0x0216 => meta.Difficulties.Span[2],
                0x0313 or 0x0314 or 0x0316 => meta.Difficulties.Span[3],
                0x0413 or 0x0414 or 0x0416 => meta.Difficulties.Span[0],
                0x0613 or 0x0614 or 0x0616 => meta.Difficulties.Span[4],
                0x0118 => meta.Difficulties.Span[6],
                0x0218 => meta.Difficulties.Span[7],
                0x0318 => meta.Difficulties.Span[8],
                0x0418 => meta.Difficulties.Span[5],
                0x0618 => meta.Difficulties.Span[9],
                _ => chart[NumericData.PlayLevel]
            };

            return chart;
        });

        return charts.ToList();
    }
}