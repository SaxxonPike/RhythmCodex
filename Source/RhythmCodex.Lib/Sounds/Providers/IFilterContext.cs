using System;
using System.Collections.Generic;

namespace RhythmCodex.Sounds.Providers;

public interface IFilterContext
{
    float[] Filter(ReadOnlySpan<float> data);
}