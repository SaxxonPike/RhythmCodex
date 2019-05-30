using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using RhythmCodex.Bms.Model;
using RhythmCodex.Charting.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Bms.Converters
{
    [Service]
    public class BmsDecoder : IBmsDecoder
    {
        private static readonly Regex MeasureRegex = new Regex("^[0-9]{3}[0-9A-F]{2}$");

        private static readonly Dictionary<string, StringData> SingularStringTags = new Dictionary<string, StringData>
        {
            {"TITLE", StringData.Title},
            {"ARTIST", StringData.Artist},
            {"GENRE", StringData.Genre},
            {"STAGEFILE", StringData.StageFile},
            {"BANNER", StringData.BannerFile},
            {"BACKBMP", StringData.BackgroundFile}
        };

        private static readonly Dictionary<string, StringData> MultiStringTags = new Dictionary<string, StringData>
        {
            {"SUBTITLE", StringData.Subtitle},
            {"SUBARTIST", StringData.Subartist},
            {"COMMENT", StringData.Comment}
        };

        private static readonly Dictionary<string, NumericData> SingularNumericTags =
            new Dictionary<string, NumericData>
            {
                {"DIFFICULTY", NumericData.Difficulty},
                {"PLAYLEVEL", NumericData.PlayLevel},
                {"PLAYER", NumericData.Player},
                {"RANK", NumericData.Rank},
                {"TOTAL", NumericData.LifeBar},
                {"VOLWAV", NumericData.Volume}
            };

        public BmsChart Decode(IEnumerable<BmsCommand> commands)
        {
            var commandList = commands.AsList();
            var chart = new Chart
            {
                Events = GetEvents(commandList).ToList()
            };

            var initialBpm = chart.Events
                .Where(c => c[NumericData.MetricOffset] == 0 && c[NumericData.Bpm] != null)
                .Select(c => c[NumericData.Bpm])
                .FirstOrDefault();

            if (initialBpm == null)
            {
                initialBpm = commandList.Where(c => "bpm".Equals(c.Name, StringComparison.OrdinalIgnoreCase))
                    .Select(c => BigRationalParser.ParseString(c.Value))
                    .First();

                if (initialBpm != null)
                    chart.Events.Add(new Event {[NumericData.Bpm] = initialBpm, [NumericData.MetricOffset] = 0});
            }

            AddSingularMetadata(chart, commandList, SingularNumericTags);
            AddSingularMetadata(chart, commandList, SingularStringTags);
            AddMultiMetadata(chart, commandList, MultiStringTags);

            return new BmsChart
            {
                Chart = chart,
                SoundMap = GetStringMap(commandList, "WAV").Map
            };
        }

        private void AddSingularMetadata(IMetadata chart, IList<BmsCommand> commandList,
            IDictionary<string, StringData> metadataMap)
        {
            foreach (var kv in metadataMap)
            {
                var command = commandList
                    .LastOrDefault(c => kv.Key.Equals(c.Name, StringComparison.InvariantCultureIgnoreCase));
                if (command != null)
                    chart[kv.Value] = command.Value;
            }
        }

        private void AddSingularMetadata(IMetadata chart, IList<BmsCommand> commandList,
            IDictionary<string, NumericData> metadataMap)
        {
            foreach (var kv in metadataMap)
            {
                var command = commandList
                    .LastOrDefault(c => kv.Key.Equals(c.Name, StringComparison.InvariantCultureIgnoreCase));
                if (command != null)
                    chart[kv.Value] = BigRationalParser.ParseString(command.Value);
            }
        }

        private void AddMultiMetadata(IMetadata chart, IList<BmsCommand> commandList,
            IDictionary<string, StringData> metadataMap)
        {
            foreach (var kv in metadataMap)
            {
                var commands = commandList
                    .Where(c => kv.Key.Equals(c.Name, StringComparison.InvariantCultureIgnoreCase))
                    .ToArray();
                if (commands.Any())
                    chart[kv.Value] = string.Join(Environment.NewLine, commands.Select(c => c.Value));
            }
        }

        private IEnumerable<IEvent> GetEvents(IEnumerable<BmsCommand> commands)
        {
            // Excluded tags (they are processed separately)

            var commandList = commands
                .Where(c => c.Name != null)
                .ToList();

            // Negotiate maps.

            var bpmMapResult = GetNumericMap(commandList, "BPM");

            // Convert commands.

            foreach (var command in commandList)
            {
                if (command.UseColon && MeasureRegex.IsMatch(command.Name))
                {
                    foreach (var ev in GetMeasureEvents(command, bpmMapResult.Map))
                        yield return ev;
                }
            }
        }

        private IEnumerable<IEvent> GetMeasureEvents(BmsCommand command, IDictionary<int, BigRational> bpmMap)
        {
            var measure = Alphabet.DecodeNumeric(command.Name.AsSpan(0, 3));
            var lane = Alphabet.DecodeHex(command.Name.AsSpan(3, 2));

            switch (lane)
            {
                case 0x02:
                {
                    return new[]
                    {
                        new Event
                        {
                            [NumericData.MeasureLength] = BigRationalParser.ParseString(command.Value),
                            [NumericData.MetricOffset] = measure
                        }
                    };
                }
            }

            var events = new List<Event>();
            var total = command.Value.Length / 2;
            for (var index = 0; index < total; index++)
            {
                var valueMemory = command.Value.AsMemory(index << 1, 2);
                var value = valueMemory.Span;
                if (value[0] == '0' && value[1] == '0')
                    continue;

                void AddEvent(Event ev)
                {
                    ev[NumericData.MetricOffset] = new BigRational(new BigInteger(measure), new BigInteger(index),
                        new BigInteger(total));
                    ev[NumericData.SourceData] = Alphabet.DecodeAlphanumeric(valueMemory.Span);
                    ev[NumericData.SourceCommand] = lane;
                    events.Add(ev);
                }

                switch (lane)
                {
                    case 0x01:
                    {
                        AddEvent(new Event
                        {
                            [NumericData.PlaySound] = Alphabet.DecodeAlphanumeric(value)
                        });
                        break;
                    }
                    case 0x03:
                    {
                        AddEvent(new Event
                        {
                            [NumericData.Bpm] = Alphabet.DecodeHex(value)
                        });
                        break;
                    }
                    case 0x04:
                    {
                        AddEvent(new Event
                        {
                            [NumericData.BgaBase] = Alphabet.DecodeAlphanumeric(value)
                        });
                        break;
                    }
                    case 0x05:
                    {
                        AddEvent(new Event
                        {
                            [NumericData.BgaObject] = Alphabet.DecodeAlphanumeric(value)
                        });
                        break;
                    }
                    case 0x06:
                    {
                        AddEvent(new Event
                        {
                            [NumericData.BgaPoor] = Alphabet.DecodeAlphanumeric(value)
                        });
                        break;
                    }
                    case 0x07:
                    {
                        AddEvent(new Event
                        {
                            [NumericData.BgaLayer] = Alphabet.DecodeAlphanumeric(value)
                        });
                        break;
                    }
                    case 0x08:
                    {
                        var number = Alphabet.DecodeAlphanumeric(value);
                        if (bpmMap.ContainsKey(number))
                        {
                            AddEvent(new Event
                            {
                                [NumericData.Bpm] = bpmMap[number]
                            });
                        }

                        break;
                    }
                    case 0x11:
                    case 0x12:
                    case 0x13:
                    case 0x14:
                    case 0x15:
                    case 0x21:
                    case 0x22:
                    case 0x23:
                    case 0x24:
                    case 0x25:
                    {
                        AddEvent(new Event
                        {
                            [NumericData.Player] = (lane & 0x20) >> 5,
                            [FlagData.Note] = true,
                            [NumericData.Column] = lane & 0x0F
                        });
                        break;
                    }
                    case 0x18:
                    case 0x19:
                    case 0x28:
                    case 0x29:
                    {
                        AddEvent(new Event
                        {
                            [NumericData.Player] = (lane & 0x20) >> 5,
                            [FlagData.Note] = true,
                            [NumericData.Column] = (lane & 0x0F) - 2
                        });
                        break;
                    }
                    case 0x16:
                    case 0x26:
                    {
                        AddEvent(new Event
                        {
                            [NumericData.Player] = (lane & 0x20) >> 5,
                            [FlagData.Scratch] = true,
                            [FlagData.Note] = true
                        });
                        break;
                    }
                    case 0x17:
                    case 0x27:
                    {
                        AddEvent(new Event
                        {
                            [NumericData.Player] = (lane & 0x20) >> 5,
                            [FlagData.FreeZone] = true,
                            [FlagData.Note] = true
                        });
                        break;
                    }
                    case 0x31:
                    case 0x32:
                    case 0x33:
                    case 0x34:
                    case 0x35:
                    case 0x41:
                    case 0x42:
                    case 0x43:
                    case 0x44:
                    case 0x45:
                    {
                        AddEvent(new Event
                        {
                            [NumericData.Player] = (lane & 0x20) >> 5,
                            [NumericData.Column] = lane & 0x0F,
                            [NumericData.LoadSound] = Alphabet.DecodeAlphanumeric(value)
                        });
                        break;
                    }
                    case 0x38:
                    case 0x39:
                    case 0x48:
                    case 0x49:
                    {
                        AddEvent(new Event
                        {
                            [NumericData.Player] = (lane & 0x20) >> 5,
                            [NumericData.Column] = (lane & 0x0F) - 2,
                            [NumericData.LoadSound] = Alphabet.DecodeAlphanumeric(value)
                        });
                        break;
                    }
                    case 0x36:
                    case 0x37:
                    case 0x46:
                    case 0x47:
                    {
                        AddEvent(new Event
                        {
                            [NumericData.Player] = (lane & 0x20) >> 5,
                            [FlagData.Scratch] = true,
                            [NumericData.LoadSound] = Alphabet.DecodeAlphanumeric(value)
                        });
                        break;
                    }
                }
            }

            return events;
        }

        private (Dictionary<int, BigRational> Map, IList<BmsCommand> Commands) GetNumericMap(
            IEnumerable<BmsCommand> commands, string mapName)
        {
            var map = new Dictionary<int, BigRational>();
            var processedCommands = new List<BmsCommand>();
            foreach (var command in commands
                .Where(c => c.Value != null &&
                            !c.Name.Equals(mapName, StringComparison.InvariantCultureIgnoreCase) &&
                            c.Name.StartsWith(mapName, StringComparison.InvariantCultureIgnoreCase)))
            {
                var index = Alphabet.DecodeAlphanumeric(command.Name.AsSpan(mapName.Length));
                var bpm = BigRationalParser.ParseString(command.Value);
                if (bpm != null)
                    map[index] = bpm.Value;
                processedCommands.Add(command);
            }

            return (map, processedCommands);
        }

        private (Dictionary<int, string> Map, IList<BmsCommand> Commands) GetStringMap(IEnumerable<BmsCommand> commands,
            string mapName)
        {
            var map = new Dictionary<int, string>();
            var processedCommands = new List<BmsCommand>();
            foreach (var command in commands
                .Where(c => c.Value != null &&
                            !c.Name.Equals(mapName, StringComparison.InvariantCultureIgnoreCase) &&
                            c.Name.StartsWith(mapName, StringComparison.InvariantCultureIgnoreCase)))
            {
                var index = Alphabet.DecodeAlphanumeric(command.Name.AsSpan(3));
                map[index] = command.Value;
                processedCommands.Add(command);
            }

            return (map, processedCommands);
        }
    }
}