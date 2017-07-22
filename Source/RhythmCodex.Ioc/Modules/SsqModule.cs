using System.Collections.Generic;
using Autofac;
using RhythmCodex.Ssq.Model;
using RhythmCodex.Ssq.Streamers;
using RhythmCodex.Streamers;

namespace RhythmCodex.Ioc.Modules
{
    public class SsqModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ChunkStreamReader>()
                .As<IStreamReader<Chunk?>>()
                .SingleInstance();

            builder.RegisterType<ChunkStreamWriter>()
                .As<IStreamWriter<Chunk?>>()
                .SingleInstance();

            builder.RegisterType<SsqStreamReader>()
                .As<IStreamReader<IEnumerable<Chunk?>>>()
                .SingleInstance();

            builder.RegisterType<SsqStreamWriter>()
                .As<IStreamWriter<IEnumerable<Chunk?>>>()
                .SingleInstance();
        }
    }
}
