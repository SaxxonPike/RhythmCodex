using Autofac;
using RhythmCodex.Ssq.Streamers;

namespace RhythmCodex.Ioc.Modules
{
    public class SsqModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ChunkStreamReader>()
                .As<IChunkStreamReader>()
                .SingleInstance();

            builder.RegisterType<ChunkStreamWriter>()
                .As<IChunkStreamWriter>()
                .SingleInstance();

            builder.RegisterType<SsqStreamReader>()
                .As<ISsqStreamReader>()
                .SingleInstance();

            builder.RegisterType<SsqStreamWriter>()
                .As<ISsqStreamWriter>()
                .SingleInstance();
        }
    }
}
