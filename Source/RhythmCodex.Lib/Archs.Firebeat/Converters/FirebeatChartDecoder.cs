using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Archs.Firebeat.Models;
using RhythmCodex.Charts.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Archs.Firebeat.Converters;

[Service]
public class FirebeatChartDecoder : IFirebeatChartDecoder
{
    public Chart Decode(IEnumerable<FirebeatChartEvent> events)
    {
        var eventList = events.AsList();

        //
        // Some charts have performances that are selected based on how
        // well the player is playing at the time. This logic should
        // always choose the set where the player is performing perfectly.
        //

        var resultEvents = new List<Event>();
        var idx = 0;

        foreach (var ev in eventList)
        {
            Event? toAdd = null;

            var performance = (ev.Player & 0xFE) >> 1;
            var player = ev.Player & 1;

            switch (ev.Type)
            {
                case 0xFF:
                    continue;

                case 0x00:
                {
                    // marker

                    var column = ev.Data & 0x000F;

                    toAdd = new Event
                    {
                        [NumericData.SourceColumn] = column,
                        [NumericData.Player] = player,
                        [FlagData.Note] = true
                    };

                    MapColumn(toAdd, column);

                    break;
                }
                case 0x02:
                {
                    // sound select

                    var column = (ev.Data & 0x0F00) >> 8;
                    var program = ev.Data & 0x00FF;

                    toAdd = new Event
                    {
                        [NumericData.SourceSound] = program,
                        [NumericData.SourceColumn] = column,
                        [NumericData.Player] = player,
                        [NumericData.LoadSound] = program >= 1 ? program : -1
                    };

                    MapColumn(toAdd, column);

                    break;
                }
                case 0x04:
                {
                    // tempo

                    toAdd = new Event
                    {
                        [NumericData.Bpm] = ev.Data
                    };

                    break;
                }
                case 0x06:
                {
                    // end of song

                    toAdd = new Event
                    {
                        [FlagData.End] = true
                    };

                    break;
                }
                case 0x07:
                {
                    // bgm

                    var program = ev.Data & 0x00FF;

                    toAdd = new Event
                    {
                        [NumericData.SourceSound] = program,
                        [NumericData.PlaySound] = program >= 1 ? program : -1
                    };

                    break;
                }
                case 0x08:
                {
                    // judgement

                    var judgeNumber = (ev.Data & 0xF000) >> 12;
                    var judgeTiming = (ev.Data & 0x0FF0) >> 4;

                    toAdd = new Event
                    {
                        [NumericData.JudgeNumber] = judgeNumber,
                        [NumericData.JudgeTiming] = judgeTiming,
                        [NumericData.Player] = player,
                    };

                    break;
                }
                case 0x09 or 0x0A:
                {
                    // judgement/tutorial related

                    break;
                }
                case 0x0B:
                {
                    // unknown
                    break;
                }
                case 0x0C:
                {
                    // measure

                    toAdd = new Event
                    {
                        [FlagData.Measure] = true
                    };

                    break;
                }
                case 0x0D:
                {
                    // performance filter

                    break;
                }
                case 0x0E:
                {
                    toAdd = new Event
                    {
                        [NumericData.PlaySound] = 0
                    };

                    break;
                }
                default:
                {
                    // marker
                    break;
                }
            }

            toAdd ??= new Event();
            toAdd[NumericData.SourceData] = ev.Data;
            toAdd[NumericData.SourceCommand] = ev.Type;
            toAdd[NumericData.SourcePlayer] = ev.Player;
            toAdd[NumericData.LinearOffset] = new BigRational(ev.Tick, 1000);
            toAdd[NumericData.SourceIndex] = idx++;
            
            if (performance != 0)
                toAdd[NumericData.Performance] = performance;

            resultEvents.Add(toAdd);
        }

        return new Chart
        {
            Events = resultEvents,
            [NumericData.Rate] = new BigRational(1000, 1)
        };

        static void MapColumn(Event toAdd, int column)
        {
            switch (column)
            {
                case >= 0 and <= 4:
                {
                    toAdd[NumericData.Column] = column;
                    break;
                }
                case 5:
                {
                    toAdd[FlagData.Scratch] = true;
                    break;
                }
                case 7:
                {
                    toAdd[FlagData.FootPedal] = true;
                    break;
                }
                case 8:
                {
                    toAdd[FlagData.FreeZone] = true;
                    break;
                }
                default:
                {
                    break;
                }
            }
        }
    }
}