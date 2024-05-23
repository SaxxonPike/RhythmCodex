using System;
using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Heuristics;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ContextAttribute(params Context[] contexts) : Attribute
{
    public IList<Context> Contexts { get; } = contexts.ToList();
}