using System;

namespace RhythmCodex.Archs.Psx.Processors;

public interface IPsxGaussianInterpolation
{
    int Interpolate(
        ReadOnlySpan<float> input,
        Span<float> output,
        float position,
        float targetRatio
    );

    float InterpolateOne(
        float s3,
        float s2,
        float s1,
        float s0,
        float position
    );
}