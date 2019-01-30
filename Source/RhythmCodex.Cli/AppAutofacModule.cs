using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using RhythmCodex.Charting;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Stepmania.Streamers;

namespace RhythmCodex.Cli
{
    /// <inheritdoc />
    public class AppAutofacModule : Autofac.Module
    {
        /// <summary>
        /// A single type from each assembly that needs to be auto-loaded.
        /// </summary>
        private static readonly IEnumerable<Type> IocTypes = new[]
        {
            typeof(App),   // RhythmCodex.Cli
            typeof(Chart)  // RhythmCodex.Lib
        };

        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var mappings = ServiceTypes.GetMappings(IocTypes.Select(t => t.Assembly).ToArray());

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
}