using System;
using System.Linq;
using System.Xml.Linq;

namespace RhythmCodex.Infrastructure;

public static class XElementExtensions
{
    public static string GetString(this XElement parent, string key)
    {
        var element = parent.Elements(key).FirstOrDefault();
        return element?.Value;
    }

    public static int? GetInt(this XElement parent, string key)
    {
        var value = parent.GetString(key);
        if (value == null)
            return null;
        return int.Parse(value);
    }

    public static int[] GetInts(this XElement parent, string key)
    {
        var values = parent.GetString(key)?
            .Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToArray();
        return values ?? Array.Empty<int>();
    }
}