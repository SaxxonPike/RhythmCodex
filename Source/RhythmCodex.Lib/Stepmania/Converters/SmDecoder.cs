using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    [Service]
    public class SmDecoder : ISmDecoder
    {
        private readonly ILogger _logger;
        private readonly INoteCommandStringDecoder _noteCommandStringDecoder;
        private readonly INoteDecoder _noteDecoder;
        private readonly ITimedCommandStringDecoder _timedCommandStringDecoder;

        public SmDecoder(
            ITimedCommandStringDecoder timedCommandStringDecoder,
            INoteCommandStringDecoder noteCommandStringDecoder,
            INoteDecoder noteDecoder,
            ILogger logger)
        {
            _timedCommandStringDecoder = timedCommandStringDecoder;
            _noteCommandStringDecoder = noteCommandStringDecoder;
            _noteDecoder = noteDecoder;
            _logger = logger;
        }

        public ChartSet Decode(IEnumerable<Command> commands)
        {
            var charts = new List<IChart>();
            var metadata = new Metadata();
            var globalEvents = new List<IEvent>();

            foreach (var command in commands)
                if (command.Name.Equals(ChartTag.BpmsTag, StringComparison.OrdinalIgnoreCase))
                {
                    globalEvents.AddRange(_timedCommandStringDecoder
                        .Decode(string.Join(",", command.Values))
                        .Select(ev => new Event
                        {
                            [NumericData.MetricOffset] = ev.Offset,
                            [NumericData.Bpm] = ev.Value
                        }));
                }
                else if (command.Name.Equals(ChartTag.StopsTag, StringComparison.OrdinalIgnoreCase))
                {
                    globalEvents.AddRange(_timedCommandStringDecoder
                        .Decode(string.Join(",", command.Values))
                        .Select(ev => new Event
                        {
                            [NumericData.MetricOffset] = ev.Offset,
                            [NumericData.Stop] = ev.Value
                        }));
                }
                else if (command.Name.Equals(ChartTag.NotesTag, StringComparison.OrdinalIgnoreCase))
                {
                    if (command.Values.Count < 6)
                    {
                        _logger.Warning(
                            $"Not enough sections in {ChartTag.NotesTag}. Need 6, found {command.Values.Count}");
                        continue;
                    }

                    var columns = GetColumnCount(command.Values[0]);
                    if (columns < 1)
                        continue;

                    var hasPlayLevel = int.TryParse(command.Values[3], out var playLevel);
                    var events = _noteCommandStringDecoder.Decode(columns, command.Values[5]);

                    charts.Add(new Chart
                    {
                        [StringData.Description] = command.Values[1],
                        [StringData.Difficulty] = command.Values[2],
                        [NumericData.PlayLevel] = hasPlayLevel ? (BigRational?) playLevel : null,
                        Events = _noteDecoder.Decode(events, columns).ToList()
                    });
                }
                else
                {
                    metadata[command.Name] = string.Join(":", command.Values);
                }

            return new ChartSet
            {
                Charts = charts,
                Metadata = metadata
            };
        }

        private int GetColumnCount(string type)
        {
            switch (type.Trim().ToLowerInvariant())
            {
                case SmGameTypes.DanceSingle:
                    return 4;
                case SmGameTypes.DanceDouble:
                case SmGameTypes.DanceCouple:
                    return 8;
                case SmGameTypes.DanceSolo:
                    return 6;
                case SmGameTypes.PumpSingle:
                case SmGameTypes.Ez2Single:
                case SmGameTypes.Ez2Double:
                case SmGameTypes.ParaSingle:
                    return 5;
                case SmGameTypes.PumpDouble:
                case SmGameTypes.PumpCouple:
                    return 10;
                case SmGameTypes.Ez2Real:
                    return 7;
                default:
                    _logger.Warning($"Unknown game type {type}.");
                    return 0;
            }
        }
    }
}