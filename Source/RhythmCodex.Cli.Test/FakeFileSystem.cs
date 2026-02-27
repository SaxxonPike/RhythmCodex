using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ClientCommon;

namespace RhythmCodex.Cli;

/// <summary>
///     A file system that doesn't actually write to disk.
/// </summary>
[ExcludeFromCodeCoverage]
public class FakeFileSystem(IFileSystem fileSystem) : IFileSystem
{
    private readonly Dictionary<string, MemoryStream> _files = new();

    /// <inheritdoc />
    public string GetFileName(string path)
    {
        return fileSystem.GetFileName(path);
    }

    /// <inheritdoc />
    public Stream OpenRead(string path)
    {
        if (!_files.TryGetValue(path, out var file))
            throw new IOException($"File not found: {path}");

        var result = new MemoryStream(file.ToArray());
        return result;
    }

    /// <inheritdoc />
    public Stream OpenWrite(string path)
    {
        if (_files.TryGetValue(path, out var file))
            file.Dispose();

        var result = new MemoryStream();
        _files[path] = result;
        return result;
    }

    /// <inheritdoc />
    public string CombinePath(params string[] paths)
    {
        return fileSystem.CombinePath(paths);
    }

    /// <inheritdoc />
    public string CurrentPath => new(Path.DirectorySeparatorChar, 1);

    /// <inheritdoc />
    public Memory<byte> ReadAllBytes(string path)
    {
        if (!_files.TryGetValue(path, out var file))
            throw new IOException($"File not found: {path}");

        return file.ToArray();
    }

    /// <inheritdoc />
    public void WriteAllBytes(string path, ReadOnlySpan<byte> data)
    {
        _files[path] = new MemoryStream(data.ToArray());
    }

    /// <inheritdoc />
    public void CreateDirectory(string path)
    {
    }

    /// <inheritdoc />
    public IEnumerable<string> GetFileNames(string? path, string pattern, bool recursive = false) =>
        path == null 
            ? [] 
            : _files
                .Where(f => f.Key.StartsWith(path, StringComparison.OrdinalIgnoreCase))
                .Select(f => f.Key);

    /// <inheritdoc />
    public IEnumerable<string> GetDirectoryNames(string path)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public string GetDirectory(string path)
    {
        return fileSystem.GetDirectory(path)!;
    }

    /// <inheritdoc />
    public string GetSafeFileName(string fileName)
    {
        return fileSystem.GetSafeFileName(fileName);
    }
}