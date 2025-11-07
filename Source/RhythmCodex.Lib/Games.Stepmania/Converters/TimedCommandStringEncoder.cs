using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Games.Stepmania.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Games.Stepmania.Converters;

[Service]
public class TimedCommandStringEncoder(INumberFormatter numberFormatter) : ITimedCommandStringEncoder
{
    public string Encode(IEnumerable<TimedEvent> events)
    {
        var places = StepmaniaConstants.DecimalPlaces;

        return string.Join(",",
            events.Select(ev =>
            {
                var key = numberFormatter.Format(ev.Offset * 4, places);
                var value = numberFormatter.Format(BigRational.IsInfinity(ev.Value) ? ev.Offset == 0 ? 99999 : -1 : ev.Value, places);
                return $"{key}={value}";
            }));
    }
}