using System;
using System.Collections.Generic;

namespace RhythmCodex.Sounds.Providers;

public interface IResampler
{
    string Name { get; }
    int Priority { get; }
    float[] Resample(ReadOnlySpan<float> data, float sourceRate, float targetRate);
}