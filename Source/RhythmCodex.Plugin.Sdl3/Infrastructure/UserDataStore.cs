using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace RhythmCodex.Plugin.Sdl3.Infrastructure;

/// <summary>
/// Implements a global mapping between UserData and objects. Since managed
/// objects are relocated as necessary by .NET, unless they are pinned, object
/// pointers cannot be used directly.
/// </summary>
[PublicAPI]
internal static class UserDataStore
{
    /// <summary>
    /// Next UserData ID to assign.
    /// </summary>
    private static long _next;

    /// <summary>
    /// Mapped items.
    /// </summary>
    private static readonly ConcurrentDictionary<long, object?> Items = [];

#pragma warning disable CA2020
    /// <summary>
    /// Retrieves the next ID.
    /// </summary>
    private static IntPtr Next()
    {
        return unchecked((IntPtr)Interlocked.Increment(ref _next));
    }
#pragma warning restore CA2020

    /// <summary>
    /// Adds a UserData mapping for an object.
    /// </summary>
    public static IntPtr Add(object? item)
    {
        var key = Next();
        Items[key] = item;
        return key;
    }

    /// <summary>
    /// Adds a UserData mapping for an object.
    /// </summary>
    public static IntPtr Add<T>(T item)
    {
        return Add((object?)item);
    }

    /// <summary>
    /// Removes the UserData mapping for an object.
    /// </summary>
    public static bool Remove(IntPtr key)
    {
        return Items.Remove(key, out _);
    }

    /// <summary>
    /// Removes the UserData mapping for an object.
    /// </summary>
    public static bool Remove<T>(IntPtr key, out T? val)
    {
        var result = Items.Remove(key, out var valObj);
        val = (T?)valObj;
        return result;
    }

    /// <summary>
    /// Removes UserData mappings for all objects of the specified type.
    /// </summary>
    public static void RemoveAll<T>()
    {
        var targets = Items.Where(i => i.Value is T).ToList();
        foreach (var target in targets)
            Items.Remove(target.Key, out _);
    }

    /// <summary>
    /// Retrieves the object for a UserData mapping.
    /// </summary>
    public static object? Get(IntPtr key)
    {
        return Items.TryGetValue(key, out var item)
            ? item
            : throw new KeyNotFoundException();
    }

    /// <summary>
    /// Retrieves the object for a UserData mapping.
    /// </summary>
    public static T? Get<T>(IntPtr key)
    {
        return Items.TryGetValue(key, out var item)
            ? (T?)item
            : throw new KeyNotFoundException();
    }

    /// <summary>
    /// Retrieves the object for a UserData mapping.
    /// This does not throw when the UserData could not be found.
    /// </summary>
    /// <returns>
    /// True only if the UserData mapping was found.
    /// </returns>
    public static bool TryGet(IntPtr key, out object? item)
    {
        return Items.TryGetValue(key, out item);
    }

    /// <summary>
    /// Retrieves the object for a UserData mapping.
    /// This does not throw when the UserData could not be found.
    /// </summary>
    /// <returns>
    /// True only if the UserData mapping was found.
    /// </returns>
    public static bool TryGet<T>(IntPtr key, out T? item)
    {
        if (!Items.TryGetValue(key, out var itemObj))
        {
            item = default;
            return false;
        }

        item = (T?)itemObj;
        return true;
    }
}