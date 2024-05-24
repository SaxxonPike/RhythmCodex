using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;

namespace RhythmCodex.IoC;

/// <summary>
/// A type mapper for RhythmCodex services. Use this with your favorite IoC container.
/// </summary>
public static class ServiceTypes
{
    /// <summary>
    /// Get the complete type map for RhythmCodex.
    /// </summary>
    public static IEnumerable<ServiceMapping> GetMappings(params Assembly[] externalAssemblies)
    {
        var myPath = Path.GetDirectoryName(typeof(ServiceTypes).Assembly.Location);

        var assemblies = new List<Assembly> {AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(myPath, "RhythmCodex.dll"))};
        var pluginAssemblies = Directory
            .GetFiles(myPath, "RhythmCodex.Plugin.*.dll")
            .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
            .ToList();
            
        assemblies.AddRange(pluginAssemblies);
        assemblies.AddRange(externalAssemblies.Where(x => !assemblies.Contains(x)).ToList());

        var serviceAttributeName = typeof(ServiceAttribute).FullName;
            
        return assemblies
            .SelectMany(assembly => assembly
                .ExportedTypes
                .Where(t => t.IsClass && !t.IsAbstract &&
                            t.GetCustomAttributes().Any(a => a.GetType().FullName == serviceAttributeName)))
            .Select(t =>
            {
                var a = t.GetCustomAttributes()
                    .First(ca => ca.GetType().FullName == serviceAttributeName);
                var at = a.GetType()
                    .GetProperty(nameof(ServiceAttribute.SingleInstance), BindingFlags.Public | BindingFlags.Instance);

                return new ServiceMapping(
                    t,
                    t.GetInterfaces().Where(i => i != typeof(IDisposable)),
                    (bool) at.GetValue(a));
            })
            .ToList();
    }

    /// <summary>
    /// Register all RhythmCodex services with a .NET DI compatible service collection.
    /// </summary>
    public static void AddRhythmCodex(this IServiceCollection serviceCollection, params Assembly[] externalAssemblies)
    {
        foreach (var mapping in GetMappings(externalAssemblies))
        {
            foreach (var service in mapping.Services)
                if (mapping.SingleInstance)
                    serviceCollection.AddSingleton(service, mapping.Implementation);
                else
                    serviceCollection.AddTransient(service, mapping.Implementation);
        }
    }
}