using System;
using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Heuristics;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ContextAttribute : Attribute
{
    public ContextAttribute(params Context[] contexts)
    {
        Contexts = contexts.ToList();
    }

    public IList<Context> Contexts { get; }
}