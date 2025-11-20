using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace RhythmCodex.Utils.Cursors;

[PublicAPI]
public static class LinqExtensions
{
    // ***************************************************

    [DebuggerStepThrough]
    public static TResult[] Select<TSource, TResult>(
        this ReadOnlySpan<TSource> span,
        Func<TSource, TResult> func)
    {
        var length = span.Length;
        var result = new TResult[length];
        for (var i = 0; i < length; i++)
            result[i] = func(span[i]);
        return result;
    }

    [DebuggerStepThrough]
    public static TResult[] Select<TSource, TResult>(
        this Span<TSource> span,
        Func<TSource, TResult> func) =>
        Select((ReadOnlySpan<TSource>)span, func);

    [DebuggerStepThrough]
    public static TResult[] Select<TSource, TResult>(
        this ReadOnlyMemory<TSource> mem,
        Func<TSource, TResult> func) =>
        mem.Span.Select(func);

    [DebuggerStepThrough]
    public static TResult[] Select<TSource, TResult>(
        this Memory<TSource> mem,
        Func<TSource, TResult> func) =>
        ((ReadOnlySpan<TSource>)mem.Span).Select(func);

    [DebuggerStepThrough]
    public static TResult[] Select<TSource, TResult>(
        this ReadOnlySpan<TSource> span,
        Func<TSource, int, TResult> func)
    {
        var length = span.Length;
        var result = new TResult[length];
        for (var i = 0; i < length; i++)
            result[i] = func(span[i], i);
        return result;
    }

    [DebuggerStepThrough]
    public static TResult[] Select<TSource, TResult>(
        this Span<TSource> span,
        Func<TSource, int, TResult> func) =>
        Select((ReadOnlySpan<TSource>)span, func);

    [DebuggerStepThrough]
    public static TResult[] Select<TSource, TResult>(
        this ReadOnlyMemory<TSource> mem,
        Func<TSource, int, TResult> func) =>
        mem.Span.Select(func);

    [DebuggerStepThrough]
    public static TResult[] Select<TSource, TResult>(
        this Memory<TSource> mem,
        Func<TSource, int, TResult> func) =>
        ((ReadOnlySpan<TSource>)mem.Span).Select(func);
    
    // ***************************************************
}