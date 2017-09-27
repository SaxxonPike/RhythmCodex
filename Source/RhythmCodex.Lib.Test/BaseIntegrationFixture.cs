using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using NUnit.Framework;
using RhythmCodex.Infrastructure;

namespace RhythmCodex
{
    /// <summary>
    ///     Base test fixture for all integration tests that use a simple container.
    /// </summary>
    public class BaseIntegrationFixture<TSubject> : BaseTestFixture
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

            builder.RegisterInstance(TestContext.Out).As<TextWriter>().SingleInstance();
            builder.Register(c => new FakeFileSystem(new FileSystem(c.Resolve<ILogger>()))).SingleInstance();
            builder.Register(c => new LoggerConfigurationSource {VerbosityLevel = LoggerVerbosityLevel.Debug})
                .As<ILoggerConfigurationSource>();
            builder.RegisterType<TextWriterLogger>().As<ILogger>();

            var assemblies = new[]
            {
                typeof(ServiceAttribute).Assembly,
                typeof(TSubject).Assembly
            };

            foreach (var assembly in assemblies)
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.GetTypeInfo().CustomAttributes.All(a => a.AttributeType == typeof(ServiceAttribute)))
                    .Except<FileSystem>()
                    .AsSelf()
                    .AsImplementedInterfaces()
                    .SingleInstance();

            return builder.Build();
        }

        /// <summary>
        ///     Gets an object from the container of the specified type.
        /// </summary>
        protected TObject Resolve<TObject>()
        {
            return Container.Resolve<TObject>();
        }
    }
}