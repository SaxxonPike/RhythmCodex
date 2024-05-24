using System.Collections.Generic;

namespace RhythmCodex.Cli.Orchestration.Infrastructure;

public interface ITaskFileFactory
{
    TaskFile CreateFromFile(string path);
    IReadOnlyList<TaskFile> CreateFromArchive(string path);
}