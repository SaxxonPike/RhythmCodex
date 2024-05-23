using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Providers;

namespace RhythmCodex.Sounds.Resamplers;

[Service]
public class FilteredSampleAndHoldResampler(IFilterProvider filterProvider) : IResampler
{
    private readonly IResampler _baseResampler = new SampleAndHoldResampler();

    public string Name => "filteredsampleandhold";

    public int Priority => int.MinValue + 1;

    public float[] Resample(ReadOnlySpan<float> data, float sourceRate, float targetRate)
    {
        var unfiltered = _baseResampler.Resample(data, sourceRate, targetRate);
        if (targetRate > sourceRate)
        {
            var filter = filterProvider.Get(FilterType.LowPass).First();
            var context = filter.Create(targetRate, sourceRate / 2);
            return context.Filter(unfiltered);
        }
        else
        {
            return unfiltered;
        }
    }
}