using System;

namespace RhythmCodex.Extensions
{
    public static class ObjectExtensions
    {
        public static TOut Use<TIn, TOut>(this TIn subj, Func<TIn, TOut> func) => func(subj);
    }
}