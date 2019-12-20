using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

#if !NET48
using System.Runtime.Loader;
#endif

namespace RhythmCodex.IoC
{
    /// <summary>
    /// A type mapper for RhythmCodex services. Use this with your favorte IoC container.
    /// </summary>
    public static class ServiceTypes
    {
        /// <summary>
        /// Get the complete type map for RhythmCodex.
        /// </summary>
        public static IEnumerable<ServiceMapping> GetMappings(params Assembly[] externalAssemblies)
        {
            var myPath = Path.GetDirectoryName(typeof(ServiceTypes).Assembly.Location);
            
            #if NET48

            var assemblies = new List<Assembly> {Assembly.LoadFile(Path.Combine(myPath, "RhythmCodex.dll"))};
            var pluginAssemblies = Directory
                .GetFiles(myPath, "RhythmCodex.Plugin.*.dll")
                .Select(Assembly.LoadFile)
                .ToList();

            #else
            
            var assemblies = new List<Assembly> {AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(myPath, "RhythmCodex.dll"))};
            var pluginAssemblies = Directory
                .GetFiles(myPath, "RhythmCodex.Plugin.*.dll")
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
                .ToList();
            
            #endif
                
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
    }
}