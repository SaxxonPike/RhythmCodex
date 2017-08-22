using System;
using System.Collections.Generic;
using System.Linq;
using Numerics;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters
{
    public class DjmainChartDecoder
    {
        public IChart Decode(IEnumerable<DjmainChartEvent> events)
        {
            return new Chart
            {
                Events = DecodeEvents(events).ToArray()
            };
        }

        private static IEnumerable<IEvent> DecodeEvents(IEnumerable<DjmainChartEvent> events)
        {
            var noteCount = true;
            
            foreach (var ev in events)
            {
                var command = ev.Param0 & 0xF;
                
                if (noteCount)
                {
                    if (ev.Offset != 0 || command != 0)
                        noteCount = false;
                    else
                        continue;
                }

                var offset = new BigRational(ev.Offset, 58);
                var param0 = ev.Param0 >> 4;
                var param1 = ev.Param1;

                switch (command)
                {
                    case 0x0:
                        switch (param0)
                        {
                            case 0xA:
                            case 0xB:
                                yield return new Event
                                {
                                    [NumericData.SourceColumn] = param0,
                                    [NumericData.LinearOffset] = offset,
                                    [FlagData.Scratch] = true,
                                    [FlagData.Note] = true,
                                    [NumericData.Player] = param0 & 1
                                };
                                break;
                            case 0xC:
                            case 0xD:
                                yield return new Event
                                {
                                    [NumericData.SourceColumn] = param0,
                                    [NumericData.LinearOffset] = offset,
                                    [FlagData.Measure] = true,
                                    [NumericData.Player] = param0 & 1
                                };
                                break;
                            case 0xE:
                            case 0xF:
                                yield return new Event
                                {
                                    [NumericData.SourceColumn] = param0,
                                    [NumericData.LinearOffset] = offset,
                                    [FlagData.FreeZone] = true,
                                    [NumericData.Player] = param0 & 1
                                };
                                break;
                            default:
                                yield return new Event
                                {
                                    [NumericData.SourceColumn] = param0,
                                    [NumericData.LinearOffset] = offset,
                                    [NumericData.Column] = param0 >> 1,
                                    [NumericData.Player] = param0 & 1,
                                    [FlagData.Note] = true
                                };
                                break;
                        }
                        break;
                    case 0x1:
                        switch (param0)
                        {
                            case 0xA:
                            case 0xB:
                                yield return new Event
                                {
                                    [NumericData.SourceColumn] = param0,
                                    [NumericData.LinearOffset] = offset,
                                    [FlagData.Scratch] = true,
                                    [NumericData.LoadSound] = param1,
                                    [NumericData.Player] = param0 & 1
                                };
                                break;
                            case 0x0:
                            case 0x1:
                            case 0x2:
                            case 0x3:
                            case 0x4:
                            case 0x5:
                            case 0x6:
                            case 0x7:
                            case 0x8:
                            case 0x9:
                                yield return new Event
                                {
                                    [NumericData.SourceColumn] = param0,
                                    [NumericData.LinearOffset] = offset,
                                    [NumericData.Column] = param0 >> 1,
                                    [NumericData.LoadSound] = param1,
                                    [NumericData.Player] = param0 & 1
                                };
                                break;
                            default:
                                yield return new Event
                                {
                                    [NumericData.SourceColumn] = param0,
                                    [NumericData.LinearOffset] = offset,
                                    [NumericData.LoadSound] = param1,
                                    [NumericData.Player] = param0 & 1

                                };
                                break;
                        }
                        break;
                    case 0x2:
                        yield return new Event
                        {
                            [NumericData.LinearOffset] = offset,
                            [NumericData.Bpm] = param1 | (param0 << 8)
                        };
                        break;
                    case 0x4:
                        yield return new Event
                        {
                            [NumericData.LinearOffset] = offset,
                            [FlagData.End] = true
                        };
                        break;
                    case 0x5:
                        yield return new Event
                        {
                            [NumericData.LinearOffset] = offset,
                            [NumericData.Panning] = new BigRational(Math.Max(param0 - 1, 0), 15),
                            [NumericData.PlaySound] = param1
                        };
                        break;
                    case 0x6:
                        yield return new Event
                        {
                            [NumericData.LinearOffset] = offset,
                            [NumericData.JudgeNumber] = param0,
                            [NumericData.JudgeTiming] = param1,
                            [NumericData.SourceColumn] = param0
                        };
                        break;
                    case 0x7:
                        yield return new Event
                        {
                            [NumericData.LinearOffset] = offset,
                            [NumericData.JudgeNumber] = param0 & 0x7,
                            [NumericData.Player] = param0 >> 3,
                            [NumericData.SourceColumn] = param0,
                            [NumericData.JudgeSound] = param1
                        };
                        break;
                    case 0x8:
                        yield return new Event
                        {
                            [NumericData.LinearOffset] = offset,
                            [NumericData.Player] = param0 >> 3,
                            [NumericData.SourceColumn] = param0,
                            [NumericData.Trigger] = param1
                        };
                        break;
                    case 0x9:
                        yield return new Event
                        {
                            [NumericData.LinearOffset] = offset,
                            [NumericData.SourceColumn] = param0,
                            [NumericData.Phrase] = param1
                        };
                        break;
                }
            }
        }
    }
}
