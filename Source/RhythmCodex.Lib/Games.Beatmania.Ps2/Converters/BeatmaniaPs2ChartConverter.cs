using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

/// <inheritdoc />
[Service]
public sealed class BeatmaniaPs2ChartConverter : IBeatmaniaPs2ChartConverter
{
    /// <inheritdoc />
    public Chart Convert(BeatmaniaPs2Chart chart)
    {
        var events = new List<Event>();
        var rateMod = BigRational.One / chart.SpeedMult;

        foreach (var ev in chart.Events)
        {
            var output = new Event
            {
                [NumericData.SourceCommand] = (int)ev.Type | (ev.Parameter << 8),
                [NumericData.SourceData] = ev.Value,
                [NumericData.SourceOffset] = ev.LinearOffset,
                [NumericData.LinearOffset] = ev.LinearOffset * chart.Rate * rateMod
            };

            events.Add(output);

            switch (ev.Type)
            {
                case BeatmaniaPs2EventType.Note1P:
                    output[NumericData.Player] = 0;
                    output[FlagData.Note] = true;
                    break;
                case BeatmaniaPs2EventType.Note2P:
                    output[NumericData.Player] = 1;
                    output[FlagData.Note] = true;
                    break;
                case BeatmaniaPs2EventType.SoundSelect1P:
                    output[NumericData.Player] = 0;
                    output[NumericData.LoadSound] = ev.Value;
                    break;
                case BeatmaniaPs2EventType.SoundSelect2P:
                    output[NumericData.Player] = 1;
                    output[NumericData.LoadSound] = ev.Value;
                    break;
                case BeatmaniaPs2EventType.Tempo:
                    output[NumericData.Bpm] = ev.Value;
                    break;
                case BeatmaniaPs2EventType.Meter:
                    output[NumericData.Meter] = ev.Value / 4;
                    break;
                case BeatmaniaPs2EventType.End:
                    output[FlagData.End] = true;
                    break;
                case BeatmaniaPs2EventType.Bgm:
                    output[NumericData.PlaySound] = ev.Value;
                    output[NumericData.Panning] = ev.Parameter;
                    break;
                case BeatmaniaPs2EventType.Judgement:
                    output[NumericData.JudgeNumber] = ev.Parameter;
                    output[NumericData.JudgeTiming] = ev.Value;
                    break;
                case BeatmaniaPs2EventType.Measure:
                    output[FlagData.Measure] = true;
                    break;
            }

            if (ev.Type is BeatmaniaPs2EventType.Note1P or BeatmaniaPs2EventType.Note2P or
                BeatmaniaPs2EventType.SoundSelect1P or BeatmaniaPs2EventType.SoundSelect2P)
            {
                switch (ev.Parameter)
                {
                    case 8:
                        output[FlagData.Scratch] = true;
                        break;
                    case < 8:
                        output[NumericData.Column] = ev.Parameter;
                        break;
                }
            }

            if (ev.Type is BeatmaniaPs2EventType.Note1P or BeatmaniaPs2EventType.Note2P && ev.Value > 0)
            {
                output[FlagData.Freeze] = true;

                events.Add(new Event
                {
                    [NumericData.LinearOffset] = output[NumericData.LinearOffset] + chart.Rate * ev.Value,
                    [NumericData.SourceOffset] = output[NumericData.SourceOffset] + ev.Value,
                    [NumericData.Player] = output[NumericData.Player],
                    [NumericData.Column] = output[NumericData.Column],
                    [FlagData.Scratch] = output[FlagData.Scratch],
                    [FlagData.FreezeEnd] = true
                });
            }
        }

        var result = new Chart
        {
            Events = events.OrderBy(x => (int)x[NumericData.SourceOffset]!).ToList(),
            [NumericData.SourceRate] = chart.Rate
        };

        return result;
    }
}