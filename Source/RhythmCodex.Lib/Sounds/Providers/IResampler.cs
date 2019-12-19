using System.Collections.Generic;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Providers
{
    public interface IResampler
    {
        string Name { get; }
        int Priority { get; }
        IList<float> Resample(IList<float> data, float sourceRate, float targetRate);
    }
}