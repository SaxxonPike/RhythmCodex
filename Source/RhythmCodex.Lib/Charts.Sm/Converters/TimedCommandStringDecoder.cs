using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charts.Sm.Model;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Charts.Sm.Converters;

[Service]
public class TimedCommandStringDecoder(ILogger logger) : ITimedCommandStringDecoder
{
    /// <inheritdoc />
    public List<TimedEvent> Decode(string events)
    {
        return Do().ToList();

        IEnumerable<TimedEvent> Do()
        {
            foreach (var ev in events.SplitEx(',').Select(s => s.Trim()))
            {
                var kv = ev.Split('=');
                if (kv.Length != 2)
                {
                    logger.Warning($"Invalid timed command key/value pair: {ev}");
                    continue;
                }

                if (!double.TryParse(kv[0], out var beat))
                {
                    logger.Warning($"Invalid offset in timed command: {ev}");
                    continue;
                }

                if (!double.TryParse(kv[1], out var value))
                {
                    logger.Warning($"Invalid value in timed command: {ev}");
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