using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
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
                    var value = _numberFormatter.Format(ev.Value, places);
                    return $"{key}={value}";
                }));
        }
    }
}