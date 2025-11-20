using System;
using RhythmCodex.IoC;

namespace RhythmCodex.Infrastructure.Tasks;

[Service]
public class TaskSpecFactory : ITaskSpecFactory
{
    public ITaskSpec<TConfiguration, TResult> CreateSpec<TConfiguration, TResult>(
        Func<ITaskContext, TConfiguration, TResult> runner
    ) => new TaskSpec<TConfiguration, TResult>(runner);
}