using System.Collections.Generic;
using System.Linq;
using RhythmCodex.IoC;

namespace RhythmCodex.Sounds.Providers;

[Service]
public class ResamplerProvider(IList<IResampler> resamplers) : IResamplerProvider
{
    public IEnumerable<IResampler> Get() => 
        resamplers;

    public IResampler Get(string name) => 
        resamplers.SingleOrDefault(x => x.Name == name);

    public IResampler GetBest() => 
        resamplers.OrderBy(x => x.Priority).Last();

    public IResampler GetFastest() =>
        resamplers.OrderBy(x => x.Priority).First();
}