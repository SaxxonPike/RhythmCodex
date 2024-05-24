using System;

namespace RhythmCodex.Sounds.Providers;

public interface IFilterContext
{
    float[] Filter(ReadOnlySpan<float> data);
}