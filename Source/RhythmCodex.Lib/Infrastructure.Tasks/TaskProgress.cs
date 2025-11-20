using JetBrains.Annotations;

namespace RhythmCodex.Infrastructure.Tasks;

/// <summary>
/// Contains a progress report for an <see cref="ITaskProcess{TResult}"/>.
/// </summary>
[PublicAPI]
public class TaskProgress
{
    /// <summary>
    /// Human-readable message for the progress report.
    /// </summary>
    public string? Message { get; set; }
    
    /// <summary>
    /// Items processed. For percentage, this should be in a range 0-100. This can be
    /// null if the progress is unknown.
    /// </summary>
    public int? Processed { get; set; }
    
    /// <summary>
    /// Total number of items. For percentage, this should be 100. This can be null
    /// if the total is unknown.
    /// </summary>
    public int? Total { get; set; }
    
    /// <summary>
    /// Inner progress data.
    /// </summary>
    public TaskProgress? SubProgress { get; set; }
};