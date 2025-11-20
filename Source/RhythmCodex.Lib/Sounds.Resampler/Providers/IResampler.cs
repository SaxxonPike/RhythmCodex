using System;

namespace RhythmCodex.Sounds.Resampler.Providers;

public interface IResampler
{
    string Name { get; }
    int Priority { get; }
    float[] Resample(ReadOnlySpan<float> data, float sourceRate, float targetRate);
}