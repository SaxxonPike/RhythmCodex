using System;
using Microsoft.Extensions.DependencyInjection;

namespace RhythmCodex;

public abstract class RhythmCodexServiceBase(IServiceProvider services)
{
    protected T Svc<T>() =>
        services.GetRequiredService<T>();
}