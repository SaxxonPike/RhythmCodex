using System;

// ReSharper disable once CheckNamespace

namespace Org.BouncyCastle.Utilities;

internal static class Platform
{
    internal static string GetTypeName(object? obj) =>
        (obj == null ? null : GetTypeName(obj.GetType())) ?? "null";

    private static string? GetTypeName(Type t) => 
        t.FullName;
}