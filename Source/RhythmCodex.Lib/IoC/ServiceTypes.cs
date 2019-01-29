using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        public static IEnumerable<ServiceMapping> GetMappings(params Assembly[] assemblies)
        {
            return assemblies
                .SelectMany(assembly => assembly
                .ExportedTypes
                .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttributes<ServiceAttribute>().Any()))
                .Select(t => new ServiceMapping(
                    t, 
                    t.GetInterfaces().Where(i => i != typeof(IDisposable)),
                    t.GetCustomAttributes<ServiceAttribute>().First().SingleInstance));
        }
    }}