using System;
using System.Collections.Generic;
using RhythmCodex.Cli.Orchestration.Infrastructure;

namespace RhythmCodex.Cli;

public interface IAppProgressTracker
{
    void Add(ITask task);
    void Remove(ITask task);
    IEnumerable<ITask> GetAll();
    void Fail(ITask task, Exception exception);
}