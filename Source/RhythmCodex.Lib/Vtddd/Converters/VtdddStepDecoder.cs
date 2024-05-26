using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Vtddd.Models;

namespace RhythmCodex.Vtddd.Converters;

[Service]
public class VtdddStepDecoder : IVtdddStepDecoder
{
    public IEnumerable<Event> Decode(VtdddStep step)
    {
        yield return new Event
        {
            [NumericData.LinearOffset] = step.Target / (BigRational)1000,
            [NumericData.Column] = step.Panel,
            [NumericData.Player] = step.Player,
            [FlagData.Note] = true
        };

        if (step.Hold)
        {
            yield return new Event
            {
                [NumericData.LinearOffset] = step.End / (BigRational)1000,
                [NumericData.Column] = step.Panel,
                [NumericData.Player] = step.Player,
                [FlagData.Freeze] = true
            };
        }
    }
}