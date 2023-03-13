using System;
using Microsoft.Extensions.DependencyInjection;

namespace RhythmCodex;

public abstract class RhythmCodexServiceBase
{
    private readonly IServiceProvider _services;

    protected RhythmCodexServiceBase(IServiceProvider services) => 
        _services = services;

    protected T Svc<T>() =>
        _services.GetRequiredService<T>();
}