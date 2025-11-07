using Microsoft.Extensions.DependencyInjection;
using RhythmCodex.FileSystems.Arc;
using RhythmCodex.Games.Beatmania;
using RhythmCodex.IoC;

namespace RhythmCodex.Plugin.DependencyInjection;

public class RhythmCodexService : IRhythmCodexService, IDisposable
{
    private readonly IServiceProvider _services;

    public RhythmCodexService()
    {
        var container = new ServiceCollection();
        container.AddRhythmCodex();
        var provider = container.BuildServiceProvider();
        _services = provider;
    }

    public RhythmCodexService(IServiceProvider services) =>
        _services = services;

    public IArcService Arc => _services.GetRequiredService<IArcService>();
    public IBeatmaniaService Beatmania => _services.GetRequiredService<IBeatmaniaService>();

    public void Dispose() =>
        (_services as IDisposable)?.Dispose();
}