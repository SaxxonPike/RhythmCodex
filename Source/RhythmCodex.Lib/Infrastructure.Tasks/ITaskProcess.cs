using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace RhythmCodex.Infrastructure.Tasks;

/// <typeparam name="TResult">
/// Type of the result object.
/// </typeparam>
[PublicAPI]
public interface ITaskProcess<TResult>
{
    /// <summary>
    /// Raised when the task has been cancelled via the cancellation token.
    /// </summary>
    event Action? Cancelled;

    /// <summary>
    /// Raised when the task has ended, whether successful or errored.
    /// </summary>
    event Action? Ended;

    /// <summary>
    /// Raised when the task has errored. Contains the exception that caused the error.
    /// </summary>
    event Action<Exception>? Errored;

    /// <summary>
    /// Raised when the task has succeeded. Contains the result object.
    /// </summary>
    event Action<TResult>? Succeeded;

    /// <summary>
    /// Raised when the task progress has updated.
    /// </summary>
    event Action<TaskProgress>? Progressed;

    /// <summary>
    /// Cancels the task.
    /// </summary>
    void Cancel();

    /// <summary>
    /// Starts the task asynchronously.
    /// </summary>
    Task<TResult> RunAsync();
}