using System;
using System.Collections.Generic;

namespace RhythmCodex.Heuristics
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ContextAttribute : Attribute
    {
        public ContextAttribute(params Context[] contexts)
        {
            Contexts = contexts;
        }

        public IEnumerable<Context> Contexts { get; }
    }
}