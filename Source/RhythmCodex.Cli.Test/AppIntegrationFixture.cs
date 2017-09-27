using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using NUnit.Framework;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli
{
    [DoNotCover]
    [Explicit]
    public class AppIntegrationFixture : BaseTestFixture
    {
        protected IContainer AppContainer { get; private set; }
        protected FakeFileSystem FileSystem => AppContainer.Resolve<FakeFileSystem>();

        [SetUp]
        public void __SetupApp()
        {
            var builder = new ContainerBuilder();
            
            builder.Register(c => new FakeFileSystem(new FileSystem(c.Resolve<ILogger>())))
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterInstance(TestContext.Out)
                .As<TextWriter>()
                .ExternallyOwned()
                .SingleInstance();
            builder.RegisterType<TextWriterLogger>()
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterType<LoggerConfigurationSource>()
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
            
            builder.RegisterModule<AppAutofacModule>();
            AppContainer = builder.Build();
        }

        [TearDown]
        public void __TeardownApp()
        {
            AppContainer.Disposer.Dispose();
            AppContainer = null;
        }
    }
}