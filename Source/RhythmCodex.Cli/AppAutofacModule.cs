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
            typeof(App),
            typeof(Chart),
            typeof(DjmainChunk),
            typeof(SsqDecoder),
            typeof(SmStreamReader)
        };

        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            foreach (var assembly in IocTypes.Select(t => t.GetTypeInfo().Assembly).Distinct())
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.GetTypeInfo().CustomAttributes.Any(a => a.AttributeType == typeof(ServiceAttribute)))
                    .AsSelf()
                    .AsImplementedInterfaces()
                    .SingleInstance();
        }
    }
}