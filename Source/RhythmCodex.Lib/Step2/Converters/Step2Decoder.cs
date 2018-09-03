using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure;
using RhythmCodex.Step2.Mappers;
using RhythmCodex.Step2.Models;

namespace RhythmCodex.Step2.Converters
{
    [Service]
    public class Step2Decoder : IStep2Decoder
    {
        private readonly IStep2EventMapper _step2EventMapper;

        public Step2Decoder(IStep2EventMapper step2EventMapper)
        {
            _step2EventMapper = step2EventMapper;
        }
        
        public IChart Decode(Step2Chunk chunk)
        {
            var stepBlocks = DecodeStepBlocks(chunk);

            var isSingleChart = stepBlocks.p1.SequenceEqual(stepBlocks.p2);
            var steps = (isSingleChart
                ? DecodeSteps(stepBlocks.p1, 0)
                : DecodeSteps(stepBlocks.p1, 0).Concat(DecodeSteps(stepBlocks.p2, 1)))
                .ToList();
            
            var events = steps
                .SelectMany(step =>
                {
                    var result = new List<IEvent>();
                    var panels = _step2EventMapper.Map(step.Panels).ToList();
                    foreach (var panel in panels)
                    {
                        result.Add(new Event
                        {
                            [NumericData.Player] = step.Player,
                            [NumericData.Column] = panel,
                            [NumericData.SourceColumn] = _step2EventMapper.Map(new[]{panel}),
                            [FlagData.Note] = true,
                            [NumericData.MetricOffset] = new BigRational(step.MetricOffset, 64)
                        });
                    }

                    return result;
                })
                .OrderBy(ev => ev[NumericData.MetricOffset])
                .ToList();

            var description = $"step2 - {events.Count(ev => ev[FlagData.Note] == true)} panels - {steps.Count(s => s.Panels != 0)} steps";
            var difficulty = "Medium";
            var type = $"dance-{(isSingleChart ? "single" : "double")}"; 

            return new Chart
            {
                Events = events,
                ["Difficulty"] = difficulty,
                ["Type"] = type,
                ["Description"] = description
            };
        }

        private IEnumerable<Step2Step> DecodeSteps(IEnumerable<byte> data, int player)
        {
            var i = 0;
            foreach (var panels in data)
            {
                if (panels != 0)
                {
                    yield return new Step2Step
                    {
                        MetricOffset = i,
                        Panels = panels,
                        Player = player
                    };
                }

                i++;
            }
        }

        private (List<byte> p1, List<byte> p2) DecodeStepBlocks(Step2Chunk chunk)
        {
            var p1 = new List<byte>();
            var p2 = new List<byte>();
            var currentP1 = 0;
            var currentP2 = 0;

            while (true)
            {
                var meta = chunk.Metadatas[currentP1];
                var data = new byte[meta.Length];
                Buffer.BlockCopy(chunk.Data, meta.Offset, data, 0, meta.Length);
                p1.AddRange(data);
                if (meta.Next1P == currentP1)
                    break;
                currentP1 = meta.Next1P;
            }
            
            while (true)
            {
                var meta = chunk.Metadatas[currentP2];
                var data = new byte[meta.Length];
                Buffer.BlockCopy(chunk.Data, meta.Offset, data, 0, meta.Length);
                p2.AddRange(data);
                if (meta.Next2P == currentP2)
                    break;
                currentP2 = meta.Next2P;
            }

            return (p1, p2);
        }
    }
}