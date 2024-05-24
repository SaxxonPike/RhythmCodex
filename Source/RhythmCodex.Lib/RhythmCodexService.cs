using System;
using Microsoft.Extensions.DependencyInjection;
using RhythmCodex.Arc;
using RhythmCodex.Beatmania;
using RhythmCodex.IoC;

namespace RhythmCodex;

public class RhythmCodexService : IRhythmCodexService, IDisposable
{
    private readonly IServiceProvider _services;
    private readonly IDisposable _disposable;

    public RhythmCodexService()
    {
        var container = new ServiceCollection();
        container.AddRhythmCodex();
        var provider = container.BuildServiceProvider();
        _disposable = provider;
        _services = provider;
    }

    public RhythmCodexService(IServiceProvider services) =>
        _services = services;

    public IArcService Arc => _services.GetRequiredService<IArcService>();
    public IBeatmaniaService Beatmania => _services.GetRequiredService<IBeatmaniaService>();

    public void Dispose() =>
        _disposable?.Dispose();
}