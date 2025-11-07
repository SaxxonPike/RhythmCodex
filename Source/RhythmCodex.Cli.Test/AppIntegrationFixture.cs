using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using ClientCommon;
using NUnit.Framework;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli;

[DoNotCover]
[Explicit]
public class AppIntegrationFixture : BaseTestFixture
{
    private static readonly ConcurrentDictionary<string, IContainer> AppContainers = [];

    protected IContainer AppContainer
    {
        get => AppContainers.TryGetValue(TestContext.CurrentContext.Test.ID, out var container)
            ? container
            : throw new InvalidOperationException("App container not found.");

        private set => AppContainers.AddOrUpdate(
            TestContext.CurrentContext.Test.ID,
            value,
            (_, container) => container
        );
    }

    protected FakeFileSystem FileSystem => AppContainer.Resolve<FakeFileSystem>();

    [SetUp]
    public void __SetupApp()
    {
        var builder = new ContainerBuilder();

        builder.Register(c => new FakeFileSystem(new PhysicalFileSystem(c.Resolve<ILogger>())))
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();
        // builder.RegisterInstance(TestContext.Out)
        //     .As<TextWriter>()
        //     .ExternallyOwned()
        //     .SingleInstance();
        builder.RegisterInstance(new TestConsole())
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();
        builder.RegisterType<TextWriterLogger>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();
        builder.RegisterType<LoggerConfigurationSource>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();
        builder.RegisterAssemblyTypes(typeof(App).Assembly)
            .Where(t => t.GetCustomAttributes<ServiceAttribute>().FirstOrDefault()?.SingleInstance ?? false)
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();
        builder.RegisterAssemblyTypes(typeof(App).Assembly)
            .Where(t => !(t.GetCustomAttributes<ServiceAttribute>().FirstOrDefault()?.SingleInstance ?? true))
            .AsSelf()
            .AsImplementedInterfaces()
            .InstancePerRequest();
        builder.RegisterAssemblyTypes(typeof(ArgParser).Assembly)
            .Where(t => t.GetCustomAttributes<ServiceAttribute>().FirstOrDefault()?.SingleInstance ?? false)
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();
        builder.RegisterAssemblyTypes(typeof(ArgParser).Assembly)
            .Where(t => !(t.GetCustomAttributes<ServiceAttribute>().FirstOrDefault()?.SingleInstance ?? true))
            .AsSelf()
            .AsImplementedInterfaces()
            .InstancePerRequest();

        builder.RegisterModule<AppAutofacModule<App>>();
        AppContainer = builder.Build();
    }

    [TearDown]
    public void __TeardownApp()
    {
        if (AppContainers.Remove(TestContext.CurrentContext.Test.ID, out var container))
            container.Dispose();
    }
}