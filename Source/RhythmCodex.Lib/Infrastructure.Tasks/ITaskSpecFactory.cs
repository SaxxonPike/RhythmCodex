using System;
using JetBrains.Annotations;

namespace RhythmCodex.Infrastructure.Tasks;

[PublicAPI]
public interface ITaskSpecFactory
{
    ITaskSpec<TConfiguration, TResult> CreateSpec<TConfiguration, TResult>(
        Func<ITaskContext, TConfiguration, TResult> runner
    );
}