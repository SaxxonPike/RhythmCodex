using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Ssq.Mappers;
using RhythmCodex.Ssq.Model;
using RhythmCodex.Step1.Models;

namespace RhythmCodex.Step1.Converters
{
    [Service]
    public class Step1Decoder : IStep1Decoder
    {
        private readonly ITimingEventDecoder _timingEventDecoder;
        private readonly IStepEventDecoder _stepEventDecoder;
        private readonly IStep1TimingChunkDecoder _timingChunkDecoder;
        private readonly IStep1StepChunkDecoder _stepChunkDecoder;
        private readonly IPanelMapperSelector _panelMapperSelector;
        private readonly IStepPanelSplitter _stepPanelSplitter;
        private readonly IStep1ChartInfoDecoder _chartInfoDecoder;

        public Step1Decoder(
            ITimingEventDecoder timingEventDecoder,
            IStepEventDecoder stepEventDecoder,
            IStep1TimingChunkDecoder timingChunkDecoder,
            IStep1StepChunkDecoder stepChunkDecoder,
            IPanelMapperSelector panelMapperSelector,
            IStepPanelSplitter stepPanelSplitter,
            IStep1ChartInfoDecoder chartInfoDecoder)
        {
            _timingEventDecoder = timingEventDecoder;
            _stepEventDecoder = stepEventDecoder;
            _timingChunkDecoder = timingChunkDecoder;
            _stepChunkDecoder = stepChunkDecoder;
            _panelMapperSelector = panelMapperSelector;
            _stepPanelSplitter = stepPanelSplitter;
            _chartInfoDecoder = chartInfoDecoder;
        }

        public IList<IChart> Decode(IEnumerable<Step1Chunk> data)
        {
            return DecodeInternal(data).ToList();
        }

        private IEnumerable<IChart> DecodeInternal(IEnumerable<Step1Chunk> data)
        {
            var chunks = data.AsList();

            if (!chunks.Any())
                throw new RhythmCodexException("No chunks to decode.");

            var timings = _timingChunkDecoder.Convert(chunks[0].Data);

            foreach (var chunk in chunks.Skip(1))
            {
                var timingEvents = _timingEventDecoder.Decode(timings);
                
                // Decode the raw steps.
                var steps = _stepChunkDecoder.Convert(chunk.Data);

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
                    panelCount = _stepPanelSplitter.Split(steps.Aggregate(0, (i, s) => i | s.Panels)).Count() / playerCount;
                }
                
                // Determine what kind of chart this is based on the panels used.
                var mapper = _panelMapperSelector.Select(steps, new ChartInfo {PanelCount = panelCount, PlayerCount = playerCount});

                // Convert the steps.
                var stepEvents = _stepEventDecoder.Decode(steps, mapper);
                var events = timingEvents.Concat(stepEvents).ToList();
                var info = _chartInfoDecoder.Decode(Bitter.ToInt32(chunk.Data.AsSpan(0)), mapper.PlayerCount, mapper.PanelCount);
                
                // Output metadata.
                var difficulty = info.Difficulty;
                var type = info.Type;
                var description = $"step1 - {events.Count(ev => ev[FlagData.Note] == true)} panels - {steps.Count(s => s.Panels != 0)} steps";
                
                // Have a chart. :3
                yield return new Chart
                {
                    Events = events,
                    [StringData.Difficulty] = difficulty,
                    [StringData.Type] = type,
                    [StringData.Description] = description
                };
            }
        }
    }
}