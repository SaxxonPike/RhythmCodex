﻿using System.Collections.Generic;

namespace RhythmCodex.Sounds.Providers
{
    public interface IResampler
    {
        string Name { get; }
        int Priority { get; }
        IList<float> Resample(IList<float> data, float sourceRate, float targetRate);
    }
}