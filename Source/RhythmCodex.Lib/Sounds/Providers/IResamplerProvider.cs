using System.Collections.Generic;

namespace RhythmCodex.Sounds.Providers;

public interface IResamplerProvider
{
    IEnumerable<IResampler> Get();
    IResampler Get(string name);
    IResampler GetBest();
    IResampler GetFastest();
}