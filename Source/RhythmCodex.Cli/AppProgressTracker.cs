using System;
using System.Collections.Generic;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli;

[Service]
public class AppProgressTracker : IAppProgressTracker
{
    private readonly List<ITask> _tasks;
    private readonly Dictionary<string, Exception> _failedTasks;

    public AppProgressTracker()
    {
        _tasks = new List<ITask>();
        _failedTasks = new Dictionary<string, Exception>();
    }

    public void Add(ITask task) => _tasks.Add(task);
    public void Remove(ITask task) => _tasks.Remove(task);
    public void Fail(ITask task, Exception e) => _failedTasks.Add(task.Id, e);
    public IEnumerable<ITask> GetAll() => _tasks;
    public IDictionary<string, Exception> GetFailed() => _failedTasks;
}