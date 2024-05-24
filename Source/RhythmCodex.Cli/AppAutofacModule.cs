using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli;

/// <inheritdoc />
public class AppAutofacModule<TFromAssembly> : Module
{
    /// <summary>
    /// A single type from each assembly that needs to be auto-loaded.
    /// </summary>
    private static readonly IEnumerable<Type> IocTypes = new[]
    {
        typeof(TFromAssembly)    // RhythmCodex.Cli
    };

    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        var mappings = ServiceTypes.GetMappings(IocTypes.Select(a => a.Assembly).ToArray());

        foreach (var mapping in mappings)
        {
            var registration = builder.RegisterType(mapping.Implementation);
            foreach (var service in mapping.Services)
                registration.As(service);
            if (mapping.SingleInstance)
                registration.SingleInstance();
            else
                registration.InstancePerDependency();
        }
    }
}