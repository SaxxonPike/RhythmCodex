using System.Collections.Generic;
using ClientCommon;

namespace RhythmCodex.Cli.Orchestration.Infrastructure;

public interface ITaskBuilder
{
}

public interface ITaskBuilder<out TTaskBuilder> : ITaskBuilder where TTaskBuilder : ITaskBuilder
{
    ITaskBuilder<TTaskBuilder> WithInputFiles(IEnumerable<string> inputFiles);
    ITaskBuilder<TTaskBuilder> WithOutputFolder(string outputFolder);
    ITaskBuilder<TTaskBuilder> WithFileSystem(IFileSystem fileSystem);
}