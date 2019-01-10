using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli.Orchestration.Infrastructure
{
    [Service]
    public class TaskFactory : ITaskFactory
    {
        private readonly Func<DdrTaskBuilder> _ddrTaskBuilderFactory;
        private readonly Func<XboxTaskBuilder> _xboxTaskBuilderFactory;
        private readonly Func<BeatmaniaTaskBuilder> _beatmaniaTaskBuilderFactory;

        public TaskFactory(
            Func<DdrTaskBuilder> ddrTaskBuilderFactory, 
            Func<XboxTaskBuilder> xboxTaskBuilderFactory,
            Func<BeatmaniaTaskBuilder> beatmaniaTaskBuilderFactory)
        {
            _ddrTaskBuilderFactory = ddrTaskBuilderFactory;
            _xboxTaskBuilderFactory = xboxTaskBuilderFactory;
            _beatmaniaTaskBuilderFactory = beatmaniaTaskBuilderFactory;
        }

        public BeatmaniaTaskBuilder BuildBeatmaniaTask() => _beatmaniaTaskBuilderFactory();
        public DdrTaskBuilder BuildDdrTask() => _ddrTaskBuilderFactory();
        public XboxTaskBuilder BuildXboxTask() => _xboxTaskBuilderFactory();
    }
}