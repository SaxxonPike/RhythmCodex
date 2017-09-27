using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using NUnit.Framework;
using RhythmCodex.Charting;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Stepmania.Streamers;

namespace RhythmCodex.Cli
{
    [DoNotCover]
    [Explicit]
    public class AppIntegrationFixture : BaseTestFixture
    {
        private static readonly IEnumerable<Type> IocTypes = new[]
        {
            typeof(App),
            typeof(Chart),
            typeof(DjmainChunk),
            typeof(SsqDecoder),
            typeof(SmStreamReader)
        };

        protected IContainer AppContainer { get; private set; }
        protected FakeFileSystem FileSystem { get; private set; }

        [SetUp]
        public void __SetupApp()
        {
            var loggerConfig = new LoggerConfigurationSource
            {
                VerbosityLevel = LoggerVerbosityLevel.Debug
            };
            var logger = new TextWriterLogger(TestContext.Out, loggerConfig);
            
            FileSystem = new FakeFileSystem(new FileSystem(logger));

            var builder = new ContainerBuilder();
            builder.RegisterInstance(TestContext.Out).As<TextWriter>();
            builder.RegisterInstance(FileSystem).As<IFileSystem>();
            builder.RegisterInstance(logger).As<ILogger>();
            builder.RegisterInstance(loggerConfig).As<ILoggerConfigurationSource>();

            foreach (var assembly in IocTypes.Select(t => t.GetTypeInfo().Assembly).Distinct())
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.GetTypeInfo().CustomAttributes.All(a => a.AttributeType == typeof(ServiceAttribute)))
                    .Except<IFileSystem>()
                    .Except<ILogger>()
                    .Except<ILoggerConfigurationSource>()
                    .AsSelf()
                    .AsImplementedInterfaces();

            AppContainer = builder.Build();
        }

        [TearDown]
        public void __TeardownApp()
        {
            FileSystem = null;
            AppContainer = null;
        }
    }
}