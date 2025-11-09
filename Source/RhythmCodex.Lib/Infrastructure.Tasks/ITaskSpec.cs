using JetBrains.Annotations;

namespace RhythmCodex.Infrastructure.Tasks;

/// <summary>
/// Describes a task.
/// </summary>
/// <typeparam name="TConfiguration">
/// Configuration object type.
/// </typeparam>
/// <typeparam name="TResult">
/// Result object type.
/// </typeparam>
[PublicAPI]
public interface ITaskSpec<in TConfiguration, TResult>
{
    /// <summary>
    /// Creates a <see cref="ITaskProcess{TResult}"/> using the specified configuration.
    /// </summary>
    /// <param name="configuration">
    /// Configuration to send to the created process.
    /// </param>
    /// <returns></returns>
    ITaskProcess<TResult> CreateProcess(
        TConfiguration configuration
    );
}