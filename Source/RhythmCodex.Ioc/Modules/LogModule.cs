using System;
using System.IO;
using Autofac;

namespace RhythmCodex.Ioc.Modules
{
    public class LogModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => Console.Out)
                .As<TextWriter>();
        }
    }
}
