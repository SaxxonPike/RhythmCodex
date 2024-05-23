using System;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Orchestration.Infrastructure;

[Service]
public class TaskFactory(
    Func<DdrTaskBuilder> ddrTaskBuilderFactory,
    Func<XboxTaskBuilder> xboxTaskBuilderFactory,
    Func<BeatmaniaTaskBuilder> beatmaniaTaskBuilderFactory,
    Func<GraphicsTaskBuilder> graphicsTaskBuilderFactory,
    Func<CompressionTaskBuilder> compressionTaskBuilderFactory,
    Func<BmsTaskBuilder> bmsTaskBuilderFactory,
    Func<ArcTaskBuilder> arcTaskBuilderFactory)
    : ITaskFactory
{
    public BeatmaniaTaskBuilder BuildBeatmaniaTask() => beatmaniaTaskBuilderFactory();
    public DdrTaskBuilder BuildDdrTask() => ddrTaskBuilderFactory();
    public XboxTaskBuilder BuildXboxTask() => xboxTaskBuilderFactory();
    public GraphicsTaskBuilder BuildGraphicsTask() => graphicsTaskBuilderFactory();
    public CompressionTaskBuilder BuildCompressionTask() => compressionTaskBuilderFactory();
    public BmsTaskBuilder BuildBmsTask() => bmsTaskBuilderFactory();
    public ArcTaskBuilder BuildArcTask() => arcTaskBuilderFactory();
}