using System;
using System.IO;

namespace RhythmCodex.Infrastructure
{
    public interface ISourceFile : IDisposable
    {
        Stream OpenRead();
        string Name { get; }
    }
}