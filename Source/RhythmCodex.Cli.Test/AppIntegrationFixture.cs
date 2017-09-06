using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using NUnit.Framework;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli
{
    public class AppIntegrationFixture : BaseIntegrationFixture
    {
        private static readonly IEnumerable<Type> IocTypes = new[]
        {
            typeof(App),
            typeof(Charting.Chart),
            typeof(Djmain.Model.DjmainChunk),
            typeof(Ssq.Converters.SsqDecoder),
            typeof(Stepmania.Streamers.SmStreamReader)
        };
        
        protected IContainer AppContainer { get; private set; }
        protected FakeFileSystem FileSystem { get; private set; }
        
        [SetUp]
        public void __SetupApp()
        {
            FileSystem = new FakeFileSystem();
            
            var builder = new ContainerBuilder();
            builder.RegisterInstance(TestContext.Out).As<TextWriter>();
            builder.RegisterInstance(FileSystem).As<IFileSystem>();
            
            foreach (var assembly in IocTypes.Select(t => t.GetTypeInfo().Assembly).Distinct())
            {
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.GetTypeInfo().CustomAttributes.All(a => a.AttributeType != typeof(ModelAttribute)))
                    .Except<FileSystem>()
                    .AsSelf()
                    .AsImplementedInterfaces();
            }
            
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