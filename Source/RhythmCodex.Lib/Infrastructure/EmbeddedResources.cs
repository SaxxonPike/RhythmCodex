using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;

// ReSharper disable MemberCanBePrivate.Global

namespace RhythmCodex.Infrastructure;

internal static class EmbeddedResources
{
    public static Stream? Open(string name, Assembly? assembly = null)
    {
        return (assembly ?? typeof(EmbeddedResources).Assembly).GetManifestResourceStream(name);
    }

    public static byte[] Get(string name, Assembly? assembly = null)
    {
        using var stream = Open(name, assembly);

        if (stream == null)
            throw new IOException($"Embedded resource {name} was not found.");

        using var mem = new MemoryStream();

        stream.CopyTo(mem);
        return mem.ToArray();
    }

    public static Dictionary<string, byte[]> GetArchive(string name)
    {
        var output = new Dictionary<string, byte[]>();

        using var mem = new MemoryStream(Get(name));
        using var archive = new ZipArchive(mem, ZipArchiveMode.Read, false);
            
        foreach (var entry in archive.Entries)
        {
            using var entryStream = entry.Open();
            using var entryCopy = new MemoryStream();

            entryStream.CopyTo(entryCopy);
            entryCopy.Flush();
            output[entry.FullName] = entryCopy.ToArray();
        }

        return output;
    }
}