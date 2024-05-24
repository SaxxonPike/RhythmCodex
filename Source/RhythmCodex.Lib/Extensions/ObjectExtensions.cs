using System;
using System.Diagnostics;

namespace RhythmCodex.Extensions;

[DebuggerStepThrough]
internal static class ObjectExtensions
{
    public static TOut Use<TIn, TOut>(this TIn subj, Func<TIn, TOut> func) => func(subj);
}