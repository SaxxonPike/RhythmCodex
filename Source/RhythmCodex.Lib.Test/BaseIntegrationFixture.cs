using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Moq;
using NUnit.Framework;
using RhythmCodex.Infrastructure;

namespace RhythmCodex
{
    public class BaseIntegrationFixture<TSubject> : BaseTestFixture
    {
        public BaseIntegrationFixture()
        {
            _container = new Lazy<IContainer>(BuildContainer);
            _subject = new Lazy<TSubject>(Resolve<TSubject>);
        }
        
        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterInstance(TestContext.Out).As<TextWriter>().SingleInstance();
            builder.RegisterInstance(new FakeFileSystem(new FileSystem())).As<IFileSystem>().SingleInstance();

            var assemblies = new[]
            {
                typeof(ServiceAttribute).Assembly,
                typeof(TSubject).Assembly
            };

            foreach (var assembly in assemblies)
            {
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.GetTypeInfo().CustomAttributes.All(a => a.AttributeType == typeof(ServiceAttribute)))
                    .Except<FileSystem>()
                    .AsSelf()
                    .AsImplementedInterfaces()
                    .SingleInstance();                
            }
            
            return builder.Build();
        }
        
        private readonly Lazy<IContainer> _container;
        protected IContainer Container => _container.Value;
        
        private readonly Lazy<TSubject> _subject;
        protected TSubject Subject => _subject.Value;

        protected TObject Resolve<TObject>() => Container.Resolve<TObject>();
    }
}
