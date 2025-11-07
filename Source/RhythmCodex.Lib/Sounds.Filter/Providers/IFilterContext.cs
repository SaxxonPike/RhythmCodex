using System;

namespace RhythmCodex.Sounds.Filter.Providers;

public interface IFilterContext
{
    float[] Filter(ReadOnlySpan<float> data);
}