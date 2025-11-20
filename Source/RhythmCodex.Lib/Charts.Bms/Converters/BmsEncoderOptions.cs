using System;

namespace RhythmCodex.Charts.Bms.Converters;

public record BmsEncoderOptions
{
    public Func<int, string?>? WavNameTransformer { get; set; }
}