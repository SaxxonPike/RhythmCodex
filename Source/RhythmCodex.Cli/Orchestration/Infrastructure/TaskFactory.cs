using System;
using RhythmCodex.Infrastructure;
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

        public TaskFactory(
            Func<DdrTaskBuilder> ddrTaskBuilderFactory, 
            Func<XboxTaskBuilder> xboxTaskBuilderFactory,
            Func<BeatmaniaTaskBuilder> beatmaniaTaskBuilderFactory,
            Func<GraphicsTaskBuilder> graphicsTaskBuilderFactory)
        {
            _ddrTaskBuilderFactory = ddrTaskBuilderFactory;
            _xboxTaskBuilderFactory = xboxTaskBuilderFactory;
            _beatmaniaTaskBuilderFactory = beatmaniaTaskBuilderFactory;
            _graphicsTaskBuilderFactory = graphicsTaskBuilderFactory;
        }

        public BeatmaniaTaskBuilder BuildBeatmaniaTask() => _beatmaniaTaskBuilderFactory();
        public DdrTaskBuilder BuildDdrTask() => _ddrTaskBuilderFactory();
        public XboxTaskBuilder BuildXboxTask() => _xboxTaskBuilderFactory();
        public GraphicsTaskBuilder BuildGraphicsTask() => _graphicsTaskBuilderFactory();
    }
}