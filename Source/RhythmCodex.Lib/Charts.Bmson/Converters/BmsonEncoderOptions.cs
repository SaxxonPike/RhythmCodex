using System;
using RhythmCodex.Charts.Bms.Converters;

namespace RhythmCodex.Charts.Bmson.Converters;

public class BmsonEncoderOptions
{
    public Func<int, string?>? WavNameTransformer { get; set; }
    public BmsChartType ChartType { get; set; }
    public string? ModeHint { get; set; }
}