using System;
using System.IO;
using Autofac;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli
{
    internal class AppInfrastructureAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            
            builder.RegisterInstance(Console.Out)
                .As<TextWriter>()
                .ExternallyOwned()
                .SingleInstance();
            builder.RegisterType<LoggerConfigurationSource>()
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();
            builder.RegisterType<TextWriterLogger>()
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();
        }
    }
}