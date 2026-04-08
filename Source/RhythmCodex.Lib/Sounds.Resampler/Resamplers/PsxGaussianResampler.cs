using System;
using System.Buffers;
using RhythmCodex.Archs.Psx.Processors;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Resampler.Providers;

namespace RhythmCodex.Sounds.Resampler.Resamplers;

[Service]
public sealed class PsxGaussianResampler(IPsxGaussianInterpolation psxGaussianInterpolation)
    : IResampler, IPsxGaussianResampler
{
    public string Name => nameof(PsxGaussianResampler);

    public int Priority => int.MinValue + 2;

    public float[] Resample(ReadOnlySpan<float> data, float sourceRate, float targetRate)
    {
        if (targetRate == sourceRate)
            return data.ToArray();
        
        var desiredLength = data.Length + 3;
        using var mem = MemoryPool<float>.Shared.Rent(data.Length + 3);
        var memSpan = mem.Memory.Span[..desiredLength];
        data.CopyTo(memSpan[3..]);
        memSpan[..3].Clear();

        var output = new float[(int)(MathF.Truncate(memSpan.Length * targetRate / sourceRate) + 1)];
        psxGaussianInterpolation.Interpolate(memSpan, output, 3, targetRate / sourceRate);

        return output;
    }
}