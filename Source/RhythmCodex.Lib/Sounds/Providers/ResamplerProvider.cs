using System.Collections.Generic;
using System.Linq;
using RhythmCodex.IoC;

namespace RhythmCodex.Sounds.Providers;

[Service]
public class ResamplerProvider : IResamplerProvider
{
    private readonly IList<IResampler> _resamplers;

    public ResamplerProvider(IList<IResampler> resamplers) => 
        _resamplers = resamplers;

    public IEnumerable<IResampler> Get() => 
        _resamplers;

    public IResampler Get(string name) => 
        _resamplers.SingleOrDefault(x => x.Name == name);

    public IResampler GetBest() => 
        _resamplers.OrderBy(x => x.Priority).Last();

    public IResampler GetFastest() =>
        _resamplers.OrderBy(x => x.Priority).First();
}