using Autofac;
using ClientCommon;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli;

internal class AppInfrastructureAutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterInstance(new Console())
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
        builder.RegisterType<PhysicalFileSystem>()
            .AsImplementedInterfaces()
            .AsSelf()
            .SingleInstance();
    }
}