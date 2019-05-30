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

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class SsqDecoder : ISsqDecoder
    {
        private readonly IChartInfoDecoder _chartInfoDecoder;
        private readonly IPanelMapperSelector _panelMapperSelector;
        private readonly ISsqChunkFilter _ssqChunkFilter;
        private readonly ISsqEventDecoder _ssqEventDecoder;

        public SsqDecoder(
            ISsqEventDecoder ssqEventDecoder,
            IPanelMapperSelector panelMapperSelector,
            IChartInfoDecoder chartInfoDecoder,
            ISsqChunkFilter ssqChunkFilter)
        {
            _ssqEventDecoder = ssqEventDecoder;
            _panelMapperSelector = panelMapperSelector;
            _chartInfoDecoder = chartInfoDecoder;
            _ssqChunkFilter = ssqChunkFilter;
        }

        public IList<IChart> Decode(IEnumerable<SsqChunk> data)
        {
            var chunks = data.AsList();

            var timings = _ssqChunkFilter.GetTimings(chunks);
            var triggers = _ssqChunkFilter.GetTriggers(chunks);
            var steps = _ssqChunkFilter.GetSteps(chunks);
            var meta = _ssqChunkFilter.GetInfos(chunks).FirstOrDefault();

            var charts = steps.Select(sc =>
            {
                var info = _chartInfoDecoder.Decode(sc.Id);
                var chart = new Chart
                {
                    Events = _ssqEventDecoder.Decode(
                            timings,
                            sc.Steps,
                            triggers,
                            _panelMapperSelector.Select(sc.Steps, info))
                        .AsList(),
                    [NumericData.Id] = sc.Id,
                    [StringData.Difficulty] = info.Difficulty,
                    [StringData.Type] = $"{SmGameTypes.Dance}-{info.Type}".ToLowerInvariant()
                };

                var firstTiming = timings.Timings.OrderBy(t => t.LinearOffset).First();
                chart[NumericData.LinearOffset] = chart.GetZeroLinearReference(
                    (BigRational) firstTiming.LinearOffset / timings.Rate,
                    (BigRational) firstTiming.MetricOffset / SsqConstants.MeasureLength);

                if (meta != null)
                {
                    chart[StringData.Title] = meta.Text[0];
                    chart[StringData.Subtitle] = meta.Text[1];
                    chart[StringData.Artist] = meta.Text[2];

                    switch (sc.Id)
                    {
                        case 0x0113:
                        case 0x0114:
                        case 0x0116:
                            chart[NumericData.PlayLevel] = meta.Difficulties[1];
                            break;
                        case 0x0213:
                        case 0x0214:
                        case 0x0216:
                            chart[NumericData.PlayLevel] = meta.Difficulties[2];
                            break;
                        case 0x0313:
                        case 0x0314:
                        case 0x0316:
                            chart[NumericData.PlayLevel] = meta.Difficulties[3];
                            break;
                        case 0x0413:
                        case 0x0414:
                        case 0x0416:
                            chart[NumericData.PlayLevel] = meta.Difficulties[0];
                            break;
                        case 0x0613:
                        case 0x0614:
                        case 0x0616:
                            chart[NumericData.PlayLevel] = meta.Difficulties[4];
                            break;
                        case 0x0118:
                            chart[NumericData.PlayLevel] = meta.Difficulties[6];
                            break;
                        case 0x0218:
                            chart[NumericData.PlayLevel] = meta.Difficulties[7];
                            break;
                        case 0x0318:
                            chart[NumericData.PlayLevel] = meta.Difficulties[8];
                            break;
                        case 0x0418:
                            chart[NumericData.PlayLevel] = meta.Difficulties[5];
                            break;
                        case 0x0618:
                            chart[NumericData.PlayLevel] = meta.Difficulties[9];
                            break;
                    }
                }

                return chart;
            });

            return charts.Cast<IChart>().AsList();
        }
    }
}