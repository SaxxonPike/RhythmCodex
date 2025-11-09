using System.Threading;
using JetBrains.Annotations;

namespace RhythmCodex.Infrastructure.Tasks;

/// <summary>
/// Provided to services that perform tasks so that progress can be reported to the caller.
/// </summary>
[PublicAPI]
public interface ITaskContext
{
    /// <summary>
    /// Token used to track task cancellation.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Reports back about a task's current progress.
    /// </summary>
    void ReportProgress(TaskProgress progress);
}