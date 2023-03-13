using Autofac;
using ClientCommon;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Gui;

internal class AppInfrastructureAutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
            
        builder.RegisterInstance(new ConsoleEventSource())
            .AsImplementedInterfaces()
            .AsSelf()
            .SingleInstance();
        builder.RegisterType<LoggerConfigurationSource>()
            .AsImplementedInterfaces()
            .AsSelf()
            .SingleInstance();
        builder.RegisterType<TextWriterLogger>()
            .AsImplementedInterfaces()
            .AsSelf()
            .SingleInstance();
        builder.RegisterType<FileSystem>()
            .AsImplementedInterfaces()
            .AsSelf()
            .SingleInstance();
    }
}