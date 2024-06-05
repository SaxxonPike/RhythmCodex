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
    public IEnumerable<Event> Decode(VtdddStep value)
    {
        yield return new Event
        {
            [NumericData.LinearOffset] = value.Target / (BigRational)1000,
            [NumericData.Column] = value.Panel,
            [NumericData.Player] = value.Player,
            [FlagData.Note] = true
        };

        if (value.Hold)
        {
            yield return new Event
            {
                [NumericData.LinearOffset] = value.End / (BigRational)1000,
                [NumericData.Column] = value.Panel,
                [NumericData.Player] = value.Player,
                [FlagData.Freeze] = true
            };
        }
    }
}