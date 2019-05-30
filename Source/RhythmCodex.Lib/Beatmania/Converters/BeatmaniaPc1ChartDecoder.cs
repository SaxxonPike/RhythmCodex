using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Beatmania.Converters
{
    [Service]
    public class BeatmaniaPc1ChartDecoder : IBeatmaniaPc1ChartDecoder
    {
        public IChart Decode(IEnumerable<BeatmaniaPc1Event> events, BigRational rate)
        {
            var result = new Chart
            {
                Events = DecodeInternal(events, rate).ToList(),
                [NumericData.Rate] = rate
            };

            return result;
        }

        private IEnumerable<IEvent> DecodeInternal(IEnumerable<BeatmaniaPc1Event> events, BigRational rate)
        {
            Event GetNewEvent(BeatmaniaPc1Event input)
            {
                return new Event
                {
                    [NumericData.SourceCommand] = input.Parameter0,
                    [NumericData.SourceColumn] = input.Parameter1,
                    [NumericData.SourceData] = input.Value,
                    [NumericData.LinearOffset] = input.LinearOffset / rate
                };
            }

            void SetPropertiesForColumn(BeatmaniaPc1Event input, IMetadata output)
            {
                output[NumericData.Player] = input.Parameter0 & 0x1;
                
                switch (input.Parameter1)
                {
                    case 0x00:
                    case 0x01:
                    case 0x02:
                    case 0x03:
                    case 0x04:
                    case 0x05:
                    case 0x06:
                    {
                        output[NumericData.Column] = input.Parameter1;
                        break;
                    }
                    case 0x07:
                    {
                        output[FlagData.Scratch] = true;
                        break;
                    }
                    case 0x08:
                    {
                        output[FlagData.FreeZone] = true;
                        break;
                    }
                }
            }

            foreach (var ev in events)
            {
                switch ((BeatmaniaPc1EventType) ev.Parameter0)
                {
                    case BeatmaniaPc1EventType.Marker1P:
                    case BeatmaniaPc1EventType.Marker2P:
                    {
                        var result = GetNewEvent(ev);
                        result[FlagData.Note] = true;
                        SetPropertiesForColumn(ev, result);

                        // freeze
                        if (ev.Value > 0)
                        {
                            result[FlagData.Freeze] = true;
                            yield return result;
                            
                            var end = GetNewEvent(ev);
                            SetPropertiesForColumn(ev, end);
                            end[FlagData.Note] = true;
                            end[FlagData.FreezeEnd] = true;
                            yield return end;
                        }
                        else
                        {
                            yield return result;
                        }
                        
                        break;
                    }
                    case BeatmaniaPc1EventType.SoundSelect1P:
                    case BeatmaniaPc1EventType.SoundSelect2P:
                    {
                        var result = GetNewEvent(ev);
                        result[NumericData.LoadSound] = ev.Value;
                        SetPropertiesForColumn(ev, result);
                        yield return result;
                        break;
                    }
                    case BeatmaniaPc1EventType.Bpm:
                    {
                        var result = GetNewEvent(ev);
                        var num = ev.Value;
                        var den = ev.Parameter1;
                        if (den < 1)
                            den = 1;
                        result[NumericData.Bpm] = new BigRational(num, den);
                        yield return result;
                        break;
                    }
                    case BeatmaniaPc1EventType.Meter:
                    {
                        var result = GetNewEvent(ev);
                        var num = ev.Value;
                        var den = ev.Parameter1;
                        if (den < 1)
                            den = 1;
                        result[NumericData.Meter] = new BigRational(num, den);
                        yield return result;
                        break;
                    }
                    case BeatmaniaPc1EventType.End:
                    {
                        var result = GetNewEvent(ev);
                        result[FlagData.End] = true;
                        yield return result;
                        break;
                    }
                    case BeatmaniaPc1EventType.Bgm:
                    {
                        var result = GetNewEvent(ev);
                        result[NumericData.PlaySound] = ev.Value;
                        yield return result;
                        break;
                    }
                    case BeatmaniaPc1EventType.JudgeTiming:
                    {
                        var result = GetNewEvent(ev);
                        result[NumericData.JudgeNumber] = ev.Parameter1;
                        result[NumericData.JudgeTiming] = ev.Value;
                        yield return result;
                        break;
                    }
                    case BeatmaniaPc1EventType.Measure:
                    {
                        var result = GetNewEvent(ev);
                        result[FlagData.Measure] = true;
                        result[NumericData.Player] = ev.Parameter1;
                        yield return result;
                        break;
                    }
                    case BeatmaniaPc1EventType.NoteCount:
                    {
                        var result = GetNewEvent(ev);
                        result[NumericData.NoteCount] = ev.Value;
                        result[NumericData.Player] = ev.Parameter1;
                        yield return result;
                        break;
                    }
                    default:
                    {
                        yield return GetNewEvent(ev);
                        break;
                    }
                }
            }
        }
    }
}