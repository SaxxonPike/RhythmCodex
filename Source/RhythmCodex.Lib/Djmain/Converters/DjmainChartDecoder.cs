using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Djmain.Converters
{
    [Service]
    public class DjmainChartDecoder : IDjmainChartDecoder
    {
        public IChart Decode(IEnumerable<IDjmainChartEvent> events)
        {
            return new Chart
            {
                Events = DecodeEvents(events).ToList()
            };
        }

        private static IEnumerable<IEvent> DecodeEvents(IEnumerable<IDjmainChartEvent> events)
        {
            var noteCount = true;

            foreach (var ev in events)
            {
                var command = ev.Param0 & 0xF;

                if (noteCount)
                    if (ev.Offset != 0 || command != 0)
                        noteCount = false;
                    else
                        continue;

                var offset = new BigRational(ev.Offset, 58);
                var param0 = ev.Param0 >> 4;
                var param1 = ev.Param1;

                switch ((DjmainEventType)command)
                {
                    case DjmainEventType.Marker:
                        switch ((DjmainColumnType)param0)
                        {
                            case DjmainColumnType.Player0Scratch:
                            case DjmainColumnType.Player1Scratch:
                                yield return new Event
                                {
                                    [NumericData.SourceCommand] = ev.Param0,
                                    [NumericData.SourceData] = ev.Param1,
                                    [NumericData.SourceColumn] = param0,
                                    [NumericData.LinearOffset] = offset,
                                    [FlagData.Scratch] = true,
                                    [FlagData.Note] = true,
                                    [NumericData.Player] = param0 & 1
                                };
                                break;
                            case DjmainColumnType.Player0Measure:
                            case DjmainColumnType.Player1Measure:
                                yield return new Event
                                {
                                    [NumericData.SourceCommand] = ev.Param0,
                                    [NumericData.SourceData] = ev.Param1,
                                    [NumericData.SourceColumn] = param0,
                                    [NumericData.LinearOffset] = offset,
                                    [FlagData.Measure] = true,
                                    [NumericData.Player] = param0 & 1
                                };
                                break;
                            case DjmainColumnType.Player0FreeScratch:
                            case DjmainColumnType.Player1FreeScratch:
                                yield return new Event
                                {
                                    [NumericData.SourceCommand] = ev.Param0,
                                    [NumericData.SourceData] = ev.Param1,
                                    [NumericData.SourceColumn] = param0,
                                    [NumericData.LinearOffset] = offset,
                                    [FlagData.FreeZone] = true,
                                    [NumericData.Player] = param0 & 1
                                };
                                break;
                            default:
                                yield return new Event
                                {
                                    [NumericData.SourceCommand] = ev.Param0,
                                    [NumericData.SourceData] = ev.Param1,
                                    [NumericData.SourceColumn] = param0,
                                    [NumericData.LinearOffset] = offset,
                                    [NumericData.Column] = param0 >> 1,
                                    [NumericData.Player] = param0 & 1,
                                    [FlagData.Note] = true
                                };
                                break;
                        }
                        break;
                    case DjmainEventType.SoundSelect:
                        switch ((DjmainColumnType)param0)
                        {
                            case DjmainColumnType.Player0FreeScratch:
                            case DjmainColumnType.Player1FreeScratch:
                            case DjmainColumnType.Player0Scratch:
                            case DjmainColumnType.Player1Scratch:
                                yield return new Event
                                {
                                    [NumericData.SourceCommand] = ev.Param0,
                                    [NumericData.SourceData] = ev.Param1,
                                    [NumericData.SourceColumn] = param0,
                                    [NumericData.LinearOffset] = offset,
                                    [FlagData.Scratch] = true,
                                    [NumericData.LoadSound] = param1 - 1,
                                    [NumericData.Player] = param0 & 1
                                };
                                break;
                            case DjmainColumnType.Player0Key0:
                            case DjmainColumnType.Player1Key0:
                            case DjmainColumnType.Player0Key1:
                            case DjmainColumnType.Player1Key1:
                            case DjmainColumnType.Player0Key2:
                            case DjmainColumnType.Player1Key2:
                            case DjmainColumnType.Player0Key3:
                            case DjmainColumnType.Player1Key3:
                            case DjmainColumnType.Player0Key4:
                            case DjmainColumnType.Player1Key4:
                                yield return new Event
                                {
                                    [NumericData.SourceCommand] = ev.Param0,
                                    [NumericData.SourceData] = ev.Param1,
                                    [NumericData.SourceColumn] = param0,
                                    [NumericData.LinearOffset] = offset,
                                    [NumericData.Column] = param0 >> 1,
                                    [NumericData.LoadSound] = param1 - 1,
                                    [NumericData.Player] = param0 & 1
                                };
                                break;
                            default:
                                yield return new Event
                                {
                                    [NumericData.SourceCommand] = ev.Param0,
                                    [NumericData.SourceData] = ev.Param1,
                                    [NumericData.SourceColumn] = param0,
                                    [NumericData.LinearOffset] = offset,
                                    [NumericData.LoadSound] = param1 - 1,
                                    [NumericData.Player] = param0 & 1
                                };
                                break;
                        }
                        break;
                    case DjmainEventType.Bpm:
                        yield return new Event
                        {
                            [NumericData.SourceCommand] = ev.Param0,
                            [NumericData.SourceData] = ev.Param1,
                            [NumericData.LinearOffset] = offset,
                            [NumericData.Bpm] = param1 | (param0 << 8)
                        };
                        break;
                    case DjmainEventType.End:
                        yield return new Event
                        {
                            [NumericData.SourceCommand] = ev.Param0,
                            [NumericData.SourceData] = ev.Param1,
                            [NumericData.LinearOffset] = offset,
                            [FlagData.End] = true
                        };
                        break;
                    case DjmainEventType.Bgm:
                        yield return new Event
                        {
                            [NumericData.SourceCommand] = ev.Param0,
                            [NumericData.SourceData] = ev.Param1,
                            [NumericData.LinearOffset] = offset,
                            [NumericData.Panning] = new BigRational(Math.Max(param0 - 1, 0), 14),
                            [NumericData.PlaySound] = param1 - 1
                        };
                        break;
                    case DjmainEventType.JudgeTiming:
                        yield return new Event
                        {
                            [NumericData.SourceCommand] = ev.Param0,
                            [NumericData.SourceData] = ev.Param1,
                            [NumericData.LinearOffset] = offset,
                            [NumericData.JudgeNumber] = param0,
                            [NumericData.JudgeTiming] = param1,
                            [NumericData.SourceColumn] = param0
                        };
                        break;
                    case DjmainEventType.JudgeSound:
                        yield return new Event
                        {
                            [NumericData.SourceCommand] = ev.Param0,
                            [NumericData.SourceData] = ev.Param1,
                            [NumericData.LinearOffset] = offset,
                            [NumericData.JudgeNumber] = param0 & 0x7,
                            [NumericData.Player] = param0 >> 3,
                            [NumericData.SourceColumn] = param0,
                            [NumericData.JudgeSound] = param1 - 1
                        };
                        break;
                    case DjmainEventType.JudgeTrigger:
                        yield return new Event
                        {
                            [NumericData.SourceCommand] = ev.Param0,
                            [NumericData.SourceData] = ev.Param1,
                            [NumericData.LinearOffset] = offset,
                            [NumericData.Player] = param0 >> 3,
                            [NumericData.SourceColumn] = param0,
                            [NumericData.Trigger] = param1
                        };
                        break;
                    case DjmainEventType.PhraseSelect:
                        yield return new Event
                        {
                            [NumericData.SourceCommand] = ev.Param0,
                            [NumericData.SourceData] = ev.Param1,
                            [NumericData.LinearOffset] = offset,
                            [NumericData.SourceColumn] = param0,
                            [NumericData.Phrase] = param1
                        };
                        break;
                    default:
                        yield return new Event
                        {
                            [NumericData.SourceCommand] = ev.Param0,
                            [NumericData.SourceData] = ev.Param1,
                            [NumericData.LinearOffset] = offset
                        };
                        break;
                }
            }
        }
    }
}