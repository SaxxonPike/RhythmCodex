using System;

namespace RhythmCodex.Cli.Orchestration.Infrastructure
{
    public interface ITask
    {
        event EventHandler<string> MessageUpdated;
        event EventHandler<float> ProgressUpdated;
        
        string Name { get; }
        float Progress { get; }
        string Message { get; }
        Func<bool> Run { get; }
    }
}