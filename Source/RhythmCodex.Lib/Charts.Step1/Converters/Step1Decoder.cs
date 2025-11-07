using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Ssq;
using RhythmCodex.Charts.Ssq.Converters;
using RhythmCodex.Charts.Ssq.Mappers;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.Charts.Step1.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Games.Stepmania.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Charts.Step1.Converters;

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
    public List<Chart> Decode(IEnumerable<Step1Chunk> data)
    {
        return DecodeInternal(data).ToList();
    }

    private IEnumerable<Chart> DecodeInternal(IEnumerable<Step1Chunk> data)
    {
        var chunks = data.AsCollection();

        if (!chunks.Any())
            throw new RhythmCodexException("No chunks to decode.");

        var timings = timingChunkDecoder.Convert(chunks.First().Data.Span);

        foreach (var chunk in chunks.Skip(1))
        {
            var timingEvents = timingEventDecoder.Decode(timings);
                
            // Decode the raw steps.
            var steps = stepChunkDecoder.Convert(chunk.Data.Span);

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
            var info = chartInfoDecoder.Decode(Bitter.ToInt32(chunk.Data), mapper.PlayerCount, mapper.PanelCount);
                
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