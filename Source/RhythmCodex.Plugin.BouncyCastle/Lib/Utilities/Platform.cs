using System;

// ReSharper disable once CheckNamespace

namespace Org.BouncyCastle.Utilities;

internal static class Platform
{
    internal static string GetTypeName(object obj)
    {
        return GetTypeName(obj.GetType());
    }

    internal static string GetTypeName(Type t)
    {
        return t.FullName;
    }
}