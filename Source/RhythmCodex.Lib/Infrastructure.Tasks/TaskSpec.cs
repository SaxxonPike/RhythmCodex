using System;
using System.Threading;
using System.Threading.Tasks;

namespace RhythmCodex.Infrastructure.Tasks;

public class TaskSpec<TConfiguration, TResult>(
    Func<ITaskContext, TConfiguration, TResult> runner
) : ITaskSpec<TConfiguration, TResult>
{
    public ITaskProcess<TResult> CreateProcess(
        TConfiguration configuration
    )
    {
        var cts = new CancellationTokenSource();

        return new TaskProcess<TResult>(
            async ctx => await Task.Run(() => runner(ctx, configuration), cts.Token),
            cts
        );
    }
}