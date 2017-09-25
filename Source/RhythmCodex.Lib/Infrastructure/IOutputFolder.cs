using System;
using System.IO;

namespace RhythmCodex.Infrastructure
{
    public interface IOutputFolder : IDisposable
    {
        string GetPath(params string[] subpaths);
        Stream OpenWrite(string path);
    }
}