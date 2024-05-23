using System.Reflection;
using Autofac;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class BaseIntegrationFixture : BaseTestFixture, IResolver
{
    private readonly Lazy<IContainer> _container;

    public BaseIntegrationFixture()
    {
        _container = new Lazy<IContainer>(BuildContainer);
    }

    private IContainer Container => _container.Value;

    private static IContainer BuildContainer()
    {
        var builder = new ContainerBuilder();

        builder.RegisterModule<TestAutofacModule>();
        builder.RegisterInstance(new TestConsole())
            .As<IConsole>();
        builder.Register(_ => new LoggerConfigurationSource {VerbosityLevel = LoggerVerbosityLevel.Debug})
            .As<ILoggerConfigurationSource>();
        builder.RegisterType<TextWriterLogger>().As<ILogger>();

        return builder.Build();
    }

    /// <summary>
    ///     Gets an object from the container of the specified type.
    /// </summary>
    protected TObject Resolve<TObject>()
        where TObject : notnull
    {
        return Container.Resolve<TObject>();
    }        
        
    TObject IResolver.Resolve<TObject>() => Resolve<TObject>();
}
    
/// <summary>
///     Base test fixture for all integration tests that use a simple container.
/// </summary>
public class BaseIntegrationFixture<TSubject> : BaseTestFixture, IResolver 
    where TSubject : notnull
{
    private readonly Lazy<IContainer> _container;
    private readonly Lazy<TSubject> _subject;

    public BaseIntegrationFixture()
    {
        _container = new Lazy<IContainer>(BuildContainer);
        _subject = new Lazy<TSubject>(Resolve<TSubject>);
    }

    private IContainer Container => _container.Value;

    /// <summary>
    ///     Gets the test subject from the container.
    /// </summary>
    protected TSubject Subject => _subject.Value;

    private static IContainer BuildContainer()
    {
        var builder = new ContainerBuilder();
            
        builder.RegisterInstance(new TestConsole())
            .As<IConsole>();
        builder.Register(_ => new LoggerConfigurationSource {VerbosityLevel = LoggerVerbosityLevel.Debug})
            .As<ILoggerConfigurationSource>();
        builder.RegisterType<TextWriterLogger>().As<ILogger>();

        var assemblies = new[]
        {
            typeof(ServiceAttribute).Assembly,
            typeof(TSubject).Assembly
        };

        foreach (var assembly in assemblies)
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.GetTypeInfo().CustomAttributes.Any(a => a.AttributeType == typeof(ServiceAttribute)))
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();

        return builder.Build();
    }

    /// <summary>
    ///     Gets an object from the container of the specified type.
    /// </summary>
    protected TObject Resolve<TObject>()
        where TObject : notnull
    {
        return Container.Resolve<TObject>();
    }

    TObject IResolver.Resolve<TObject>() => Resolve<TObject>();
}