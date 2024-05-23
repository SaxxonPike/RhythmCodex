using System;
using System.IO;

namespace RhythmCodex.Cli.Orchestration.Infrastructure;

public class TaskFile
{
    public string Path { get; set; }
    public string FileName { get; set; }
    public Func<Stream> Open { get; set; }
}