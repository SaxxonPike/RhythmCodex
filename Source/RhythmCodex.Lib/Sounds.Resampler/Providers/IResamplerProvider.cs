using System.Collections.Generic;

namespace RhythmCodex.Sounds.Resampler.Providers;

public interface IResamplerProvider
{
    IEnumerable<IResampler> Get();
    IResampler Get(string name);
    IResampler GetBest();
    IResampler GetFastest();
}