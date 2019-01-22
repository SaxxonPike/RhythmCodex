using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    [Service]
    public class TimedCommandStringDecoder : ITimedCommandStringDecoder
    {
        private readonly ILogger _logger;

        public TimedCommandStringDecoder(ILogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public IEnumerable<TimedEvent> Decode(string events)
        {
            foreach (var ev in events.SplitEx(',').Select(s => s.Trim()))
            {
                var kv = ev.Split('=');
                if (kv.Length != 2)
                {
                    _logger.Warning($"Invalid timed command key/value pair: {ev}");
                    continue;
                }

                if (!double.TryParse(kv[0], out var beat))
                {
                    _logger.Warning($"Invalid offset in timed command: {ev}");
                    continue;
                }

                if (!double.TryParse(kv[1], out var value))
                {
                    _logger.Warning($"Invalid value in timed command: {ev}");
                    continue;
                }

                yield return new TimedEvent
                {
                    Offset = new BigRational(beat) / 4,
                    Value = new BigRational(value)
                };
            }
        }
    }
}