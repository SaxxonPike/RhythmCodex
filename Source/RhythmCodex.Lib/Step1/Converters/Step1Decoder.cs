using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Ssq;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Ssq.Mappers;
using RhythmCodex.Ssq.Model;
using RhythmCodex.Step1.Models;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Step1.Converters;

[Service]
public class Step1Decoder(
    ITimingEventDecoder timingEventDecoder,
    IStepEventDecoder stepEventDecoder,
    IStep1TimingChunkDecoder timingChunkDecoder,
    IStep1StepChunkDecoder stepChunkDecoder,
    IPanelMapperSelector panelMapperSelector,
    IStepPanelSplitter stepPanelSplitter,
    IStep1ChartInfoDecoder chartInfoDecoder)
    : IStep1Decoder
{
    public List<Chart> Decode(IReadOnlyCollection<Step1Chunk> data)
    {
        return DecodeInternal(data).ToList();
    }

    private IEnumerable<Chart> DecodeInternal(IReadOnlyCollection<Step1Chunk> data)
    {
        var chunks = data;

        if (!chunks.Any())
            throw new RhythmCodexException("No chunks to decode.");

        var timings = timingChunkDecoder.Convert(chunks.First().Data);

        foreach (var chunk in chunks.Skip(1))
        {
            var timingEvents = timingEventDecoder.Decode(timings);
                
            // Decode the raw steps.
            var steps = stepChunkDecoder.Convert(chunk.Data);

            // Old charts store singles charts twice, as if it was a couples chart. So, check for that.
            int? panelCount = null;
            var steps1 = steps.Select(s => s.Panels & 0xF).ToArray();
            var steps2 = steps.Select(s => s.Panels >> 4).ToArray();
            var isSingle = steps1.SequenceEqual(steps2);
            int? playerCount;
            if (isSingle)
            {
                playerCount = 1;
                panelCount = 4;
                foreach (var step in steps)
                    step.Panels &= 0xF;
            }
            else
            {
                // Bit of a hack to make solo charts work.
                playerCount = steps.Any(s => (s.Panels & 0xA0) != 0) ? 2 : 1;
                panelCount = stepPanelSplitter.Split(steps.Aggregate(0, (i, s) => i | s.Panels)).Count() / playerCount;
            }
                
            // Determine what kind of chart this is based on the panels used.
            var mapper = panelMapperSelector.Select(steps, new ChartInfo {PanelCount = panelCount, PlayerCount = playerCount});

            // Convert the steps.
            var stepEvents = stepEventDecoder.Decode(steps, mapper);
            var events = timingEvents.Concat(stepEvents).ToList();
            var info = chartInfoDecoder.Decode(Bitter.ToInt32(chunk.Data.AsSpan(0)), mapper.PlayerCount, mapper.PanelCount);
                
            // Output metadata.
            var difficulty = info.Difficulty;
            var type = $"{SmGameTypes.Dance}-{info.Type}";
            var description = $"step1 - {events.Count(ev => ev[FlagData.Note] == true)} panels - {steps.Count(s => s.Panels != 0)} steps";

            var chart = new Chart
            {
                Events = events,
                [StringData.Difficulty] = difficulty,
                [StringData.Type] = type,
                [StringData.Description] = description
            };
                
            var firstTiming = timings.Timings.OrderBy(t => t.LinearOffset).First();
            chart[NumericData.LinearOffset] = chart.GetZeroLinearReference(
                (BigRational) firstTiming.LinearOffset / timings.Rate,
                (BigRational) firstTiming.MetricOffset / SsqConstants.MeasureLength);
                
            // Have a chart. :3
            yield return chart;
        }
    }
}