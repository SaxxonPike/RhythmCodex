using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli.Orchestration.Infrastructure
{
    [Service]
    public class TaskFactory : ITaskFactory
    {
        private readonly Func<DdrTaskBuilder> _ddrTaskBuilderFactory;
        private readonly Func<XboxTaskBuilder> _xboxTaskBuilderFactory;

        public TaskFactory(
            Func<DdrTaskBuilder> ddrTaskBuilderFactory, 
            Func<XboxTaskBuilder> xboxTaskBuilderFactory)
        {
            _ddrTaskBuilderFactory = ddrTaskBuilderFactory;
            _xboxTaskBuilderFactory = xboxTaskBuilderFactory;
        }
        
        public DdrTaskBuilder BuildDdrTask() => _ddrTaskBuilderFactory();
        public XboxTaskBuilder BuildXboxTask() => _xboxTaskBuilderFactory();
    }
}