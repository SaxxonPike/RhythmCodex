using System;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Orchestration.Infrastructure
{
    [Service]
    public class TaskFactory : ITaskFactory
    {
        private readonly Func<DdrTaskBuilder> _ddrTaskBuilderFactory;
        private readonly Func<XboxTaskBuilder> _xboxTaskBuilderFactory;
        private readonly Func<BeatmaniaTaskBuilder> _beatmaniaTaskBuilderFactory;
        private readonly Func<GraphicsTaskBuilder> _graphicsTaskBuilderFactory;
        private readonly Func<CompressionTaskBuilder> _compressionTaskBuilderFactory;
        private readonly Func<BmsTaskBuilder> _bmsTaskBuilderFactory;

        public TaskFactory(
            Func<DdrTaskBuilder> ddrTaskBuilderFactory, 
            Func<XboxTaskBuilder> xboxTaskBuilderFactory,
            Func<BeatmaniaTaskBuilder> beatmaniaTaskBuilderFactory,
            Func<GraphicsTaskBuilder> graphicsTaskBuilderFactory,
            Func<CompressionTaskBuilder> compressionTaskBuilderFactory,
            Func<BmsTaskBuilder> bmsTaskBuilderFactory)
        {
            _ddrTaskBuilderFactory = ddrTaskBuilderFactory;
            _xboxTaskBuilderFactory = xboxTaskBuilderFactory;
            _beatmaniaTaskBuilderFactory = beatmaniaTaskBuilderFactory;
            _graphicsTaskBuilderFactory = graphicsTaskBuilderFactory;
            _compressionTaskBuilderFactory = compressionTaskBuilderFactory;
            _bmsTaskBuilderFactory = bmsTaskBuilderFactory;
        }

        public BeatmaniaTaskBuilder BuildBeatmaniaTask() => _beatmaniaTaskBuilderFactory();
        public DdrTaskBuilder BuildDdrTask() => _ddrTaskBuilderFactory();
        public XboxTaskBuilder BuildXboxTask() => _xboxTaskBuilderFactory();
        public GraphicsTaskBuilder BuildGraphicsTask() => _graphicsTaskBuilderFactory();
        public CompressionTaskBuilder BuildCompressionTask() => _compressionTaskBuilderFactory();
        public BmsTaskBuilder BuildBmsTask() => _bmsTaskBuilderFactory();
    }
}