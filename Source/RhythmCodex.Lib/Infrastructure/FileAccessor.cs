using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Infrastructure;

public class FileAccessor(string basePath) : IFileAccessor
{
    public bool FileExists(string name) =>
        File.Exists(Path.Combine(basePath, name));

    public Stream OpenRead(string name) =>
        File.OpenRead(Path.Combine(basePath, name));

    public ExtensionMatchedFile GetFileNameByExtension(string name, IEnumerable<string> extensions)
    {
        if (name == null)
            throw new RhythmCodexException("File name cannot be null.");

        var path = Path.Combine(basePath, name);
        var extensionsList = extensions;
        foreach (var e in extensionsList)
        {
            if (path.EndsWith($".{e}", StringComparison.InvariantCultureIgnoreCase))
                return new ExtensionMatchedFile
                {
                    Filename = name,
                    Extension = e
                };
        }

        foreach (var e in extensionsList)
        {
            var newPath = $"{Path.Combine(basePath, Path.GetDirectoryName(path) ?? ".", Path.GetFileNameWithoutExtension(path))}.{e}";
            var newName = $"{Path.Combine(Path.GetDirectoryName(name) ?? "", Path.GetFileNameWithoutExtension(path))}.{e}";
            if (File.Exists(newPath))
                return new ExtensionMatchedFile
                {
                    Filename = newName,
                    Extension = e
                };
        }

        return null;
    }
}