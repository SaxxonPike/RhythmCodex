using System;
using System.Threading;
using System.Threading.Tasks;

namespace RhythmCodex.Infrastructure.Tasks;

public class TaskProcess<TResult> : ITaskProcess<TResult>, ITaskContext
{
    private readonly CancellationTokenSource _cancellationTokenSource;

    public event Action? Cancelled;
    public event Action? Ended;
    public event Action<Exception>? Errored;
    public event Action<TResult>? Succeeded;
    public event Action<TaskProgress>? Progressed;

    public Task<TResult> Task { get; }

    public TaskProcess(
        Func<ITaskContext, Task<TResult>> runner,
        CancellationTokenSource cancellationTokenSource
    )
    {
        _cancellationTokenSource = cancellationTokenSource;
        Task = runner(this)
            .ContinueWith(t =>
            {
                if (t.IsCanceled || _cancellationTokenSource.IsCancellationRequested)
                    Cancelled?.Invoke();
                else if (t.IsCompletedSuccessfully)
                    Succeeded?.Invoke(t.Result);
                else if (t.IsFaulted)
                    Errored?.Invoke(t.Exception);
                Ended?.Invoke();

                return t.Result;
            });
    }

    public CancellationToken CancellationToken =>
        _cancellationTokenSource.Token;

    public void Cancel() =>
        _cancellationTokenSource.Cancel();

    public Task<TResult> RunAsync()
    {
        if (Task.Status == TaskStatus.Created)
            Task.Start();
        return Task;
    }

    public void ReportProgress(TaskProgress progress) =>
        Progressed?.Invoke(progress);
}