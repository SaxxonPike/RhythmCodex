using System;
using System.Collections.Generic;

namespace RhythmCodex.Cli
{
    public interface ICommand
    {
        string Description { get; }
        Action<IDictionary<string, string[]>> Execute { get; }
        string Name { get; }
    }
}