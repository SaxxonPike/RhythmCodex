using System;

namespace RhythmCodex.Sounds.Resampler.Resamplers;

public interface IPsxGaussianResampler
{
    float[] Resample(ReadOnlySpan<float> data, float sourceRate, float targetRate);
}