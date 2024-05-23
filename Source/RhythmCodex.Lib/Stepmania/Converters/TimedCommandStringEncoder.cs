using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters;

[Service]
public class TimedCommandStringEncoder : ITimedCommandStringEncoder
{
    private readonly INumberFormatter _numberFormatter;

    public TimedCommandStringEncoder(INumberFormatter numberFormatter)
    {
        _numberFormatter = numberFormatter;
    }

    public string Encode(IEnumerable<TimedEvent> events)
    {
        var places = StepmaniaConstants.DecimalPlaces;

        return string.Join(",",
            events.Select(ev =>
            {
                var key = _numberFormatter.Format(ev.Offset * 4, places);
                var value = _numberFormatter.Format(BigRational.IsInfinity(ev.Value) ? (ev.Offset == 0 ? 99999 : -1) : ev.Value, places);
                return $"{key}={value}";
            }));
    }
}