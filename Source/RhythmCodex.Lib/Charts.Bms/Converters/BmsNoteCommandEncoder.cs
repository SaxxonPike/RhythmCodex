using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using RhythmCodex.Charts.Bms.Model;
using RhythmCodex.Charts.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Charts.Bms.Converters;

[Service]
public class BmsNoteCommandEncoder(IQuantizer quantizer) : IBmsNoteCommandEncoder
{
    public string Encode(IEnumerable<BmsEvent> events, Func<BigRational?, string> encodeValue,
        BigRational measureLength, int quantize)
    {
        // round up and multiply for longer measures (100% minimoo-G would be a nightmare otherwise)
        var maxQ = Math.Max(quantize, (int)((measureLength + BigRational.OneHalf).GetWholePart() * quantize));
        var q = quantizer.GetQuantization(events.Select(e => e.Offset), BigInteger.One, maxQ);

        var buffer = Enumerable.Range(0, q).Select(_ => (BigRational?)null).ToArray();
        foreach (var ev in events)
        {
            var i = (int)(ev.Offset * q);
            buffer[i] = ev.Value;
        }

        var builder = new StringBuilder();
        foreach (var i in buffer)
            builder.Append(encodeValue(i));

        return builder.ToString();
    }

    public List<BmsEvent> TranslateNoteEvents(IEnumerable<Event> events, BmsChartType chartType)
    {
        return Do().ToList();

        IEnumerable<BmsEvent> Do()
        {
            var sounds = new Dictionary<(int player, int column, bool scratch), BigRational>();
            var freezes = new HashSet<(int player, int column, bool scratch)>();

            foreach (var ev in events.OrderBy(ev => ev[NumericData.MetricOffset]))
            {
                var offset = ev[NumericData.MetricOffset]?.GetFractionPart() ?? 0;
                var measure = (int)(ev[NumericData.MetricOffset]?.GetWholePart() ?? 0);
                var playerId = (int)(ev[NumericData.Player] ?? -1);
                var columnId = (int)(ev[NumericData.Column] ?? -1);
                var scratch = (ev[FlagData.Scratch] ?? false) || (ev[FlagData.FreeZone] ?? false);

                // Non note:

                if (ev[FlagData.Note] != true)
                {
                    if (ev[NumericData.LoadSound] != null)
                        sounds[(playerId, columnId, scratch)] = ev[NumericData.LoadSound]!.Value;

                    if (ev[NumericData.PlaySound] != null)
                    {
                        yield return new BmsEvent
                        {
                            Lane = "01",
                            Offset = offset,
                            Measure = measure,
                            Value = ev[NumericData.PlaySound]!.Value
                        };
                    }
                }

                // Note only:

                else
                {
                    var isFreeze = freezes.Contains((playerId, columnId, scratch));

                    if (ev[FlagData.Freeze] == true)
                    {
                        freezes.Add((playerId, columnId, scratch));
                        isFreeze = true;
                    }

                    var lane = GetLane(ev, isFreeze);

                    if (lane != null)
                    {
                        yield return new BmsEvent
                        {
                            Lane = lane,
                            Measure = measure,
                            Offset = offset,
                            Value = sounds.ContainsKey((playerId, columnId, scratch))
                                ? sounds[(playerId, columnId, scratch)]
                                : 1295 // TODO: ugly constant
                        };
                    }

                    if (ev[FlagData.Freeze] != true)
                    {
                        freezes.Remove((playerId, columnId, scratch));
                    }
                }
            }

            yield break;

            string? GetLane(IMetadata ev, bool isFreeze)
            {
                switch (chartType)
                {
                    case BmsChartType.Beatmania:
                    {
                        if (ev[NumericData.Player] == 0 && ev[NumericData.Column] == 0)
                            return isFreeze ? "51" : "11";
                        if (ev[NumericData.Player] == 0 && ev[NumericData.Column] == 1)
                            return isFreeze ? "52" : "12";
                        if (ev[NumericData.Player] == 0 && ev[NumericData.Column] == 2)
                            return isFreeze ? "53" : "13";
                        if (ev[NumericData.Player] == 0 && ev[NumericData.Column] == 3)
                            return isFreeze ? "54" : "14";
                        if (ev[NumericData.Player] == 0 && ev[NumericData.Column] == 4)
                            return isFreeze ? "55" : "15";
                        if (ev[NumericData.Player] == 0 && ev[NumericData.Column] == 5)
                            return isFreeze ? "58" : "18";
                        if (ev[NumericData.Player] == 0 && ev[NumericData.Column] == 6)
                            return isFreeze ? "59" : "19";
                        if (ev[NumericData.Player] == 0 && ev[FlagData.Scratch] == true)
                            return isFreeze ? "56" : "16";
                        if (ev[NumericData.Player] == 0 && ev[FlagData.FreeZone] == true)
                            return "17";
                        if (ev[NumericData.Player] == 1 && ev[NumericData.Column] == 0)
                            return isFreeze ? "61" : "21";
                        if (ev[NumericData.Player] == 1 && ev[NumericData.Column] == 1)
                            return isFreeze ? "62" : "22";
                        if (ev[NumericData.Player] == 1 && ev[NumericData.Column] == 2)
                            return isFreeze ? "63" : "23";
                        if (ev[NumericData.Player] == 1 && ev[NumericData.Column] == 3)
                            return isFreeze ? "64" : "24";
                        if (ev[NumericData.Player] == 1 && ev[NumericData.Column] == 4)
                            return isFreeze ? "65" : "25";
                        if (ev[NumericData.Player] == 1 && ev[NumericData.Column] == 5)
                            return isFreeze ? "68" : "28";
                        if (ev[NumericData.Player] == 1 && ev[NumericData.Column] == 6)
                            return isFreeze ? "69" : "29";
                        if (ev[NumericData.Player] == 1 && ev[FlagData.Scratch] == true)
                            return isFreeze ? "66" : "26";
                        if (ev[NumericData.Player] == 1 && ev[FlagData.FreeZone] == true)
                            return "27";
                        break;
                    }
                    case BmsChartType.Popn:
                    {
                        if (ev[NumericData.Player] == 0 && ev[NumericData.Column] == 0)
                            return isFreeze ? "51" : "11";
                        if (ev[NumericData.Player] == 0 && ev[NumericData.Column] == 1)
                            return isFreeze ? "52" : "12";
                        if (ev[NumericData.Player] == 0 && ev[NumericData.Column] == 2)
                            return isFreeze ? "53" : "13";
                        if (ev[NumericData.Player] == 0 && ev[NumericData.Column] == 3)
                            return isFreeze ? "54" : "14";
                        if (ev[NumericData.Player] == 0 && ev[NumericData.Column] == 4)
                            return isFreeze ? "55" : "15";
                        if (ev[NumericData.Player] == 0 && ev[NumericData.Column] == 5)
                            return isFreeze ? "62" : "22";
                        if (ev[NumericData.Player] == 0 && ev[NumericData.Column] == 6)
                            return isFreeze ? "63" : "23";
                        if (ev[NumericData.Player] == 0 && ev[NumericData.Column] == 7)
                            return isFreeze ? "64" : "24";
                        if (ev[NumericData.Player] == 0 && ev[NumericData.Column] == 8)
                            return isFreeze ? "65" : "25";
                        break;
                    }
                }

                return null;
            }
        }
    }

    public List<BmsEvent> TranslateBpmEvents(IEnumerable<Event> events)
    {
        return Do().ToList();

        IEnumerable<BmsEvent> Do()
        {
            foreach (var ev in events.OrderBy(ev => ev[NumericData.MetricOffset]))
            {
                if (ev[NumericData.Bpm] == null)
                    continue;

                var offset = ev[NumericData.MetricOffset]?.GetFractionPart() ?? 0;
                var measure = (int)(ev[NumericData.MetricOffset]?.GetWholePart() ?? 0);

                yield return new BmsEvent
                {
                    Lane = "08",
                    Measure = measure,
                    Offset = offset,
                    Value = ev[NumericData.Bpm] ?? 0
                };
            }
        }
    }
}