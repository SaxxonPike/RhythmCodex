using System;

namespace RhythmCodex.Infrastructure;

public static class WeakReferenceExtensions
{
    public static T GetOrRefresh<T>(this WeakReference<T> reference, Func<T> func)
        where T : class
    {
        if (reference.TryGetTarget(out var target))
            return target;

        target = func();
        reference.SetTarget(target);
        return target;
    }
}