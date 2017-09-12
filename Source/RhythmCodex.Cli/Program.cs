using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Program
    {
        private static readonly IEnumerable<Type> IocTypes = new[]
        {
            typeof(App),
            typeof(Charting.Chart),
            typeof(Djmain.Model.DjmainChunk),
            typeof(Ssq.Converters.SsqDecoder),
            typeof(Stepmania.Streamers.SmStreamReader)
        };
        
        public static void Main(string[] args)
        {
            BuildContainer().Resolve<IApp>().Run(args);
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(Console.Out).As<TextWriter>();

            foreach (var assembly in IocTypes.Select(t => t.GetTypeInfo().Assembly).Distinct())
            {
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.GetTypeInfo().CustomAttributes.All(a => a.AttributeType == typeof(ServiceAttribute)))
                    .AsImplementedInterfaces();                
            }
            
            return builder.Build();
        }
    }
}
