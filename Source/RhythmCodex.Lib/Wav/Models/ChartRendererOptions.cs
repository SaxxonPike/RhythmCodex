﻿using RhythmCodex.Infrastructure;

namespace RhythmCodex.Wav.Models;

[Model]
public class ChartRendererOptions
{
    public BigRational SampleRate { get; set; } = 44100;
    public BigRational? Volume { get; set; } = 1;
    public bool UseSourceDataForSamples { get; set; }
}