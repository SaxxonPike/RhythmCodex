using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace ClientCommon;

[Service]
public class FileSystem(ILogger logger) 
    : IFileSystem
{
    private const char SafeChar = '_';

    private readonly char[] _invalidChars = Path
        .GetInvalidFileNameChars()
        .Concat(Path.GetInvalidPathChars())
        .Distinct()
        .ToArray();

    /// <inheritdoc />
    public string GetFileName(string path)
    {
        return Path.GetFileName(path);
    }

    /// <inheritdoc />
    public Stream OpenRead(string path)
    {
        logger.Debug($"Open for read: {path}");
        return File.OpenRead(path);
    }

    /// <inheritdoc />
    public Stream OpenWrite(string path)
    {
        logger.Debug($"Open for write: {path}");
        BuildPathIfNotExists(path);
        return File.Open(path, FileMode.Create, FileAccess.ReadWrite);
    }

    private void BuildPathIfNotExists(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(dir) || Directory.Exists(dir))
            return;

        Directory.CreateDirectory(dir);
    }

    /// <inheritdoc />
    public string CombinePath(params string[] paths)
    {
        return Path.Combine(paths);
    }

    /// <inheritdoc />
    public string CurrentPath => Directory.GetCurrentDirectory();

    /// <inheritdoc />
    public byte[] ReadAllBytes(string path)
    {
        logger.Debug($"Reading all bytes: {path}");
        return File.ReadAllBytes(path);
    }

    /// <inheritdoc />
    public void WriteAllBytes(string path, ReadOnlySpan<byte> data)
    {
        logger.Debug($"Writing all bytes: {path}");
        using var stream = File.Open(path, FileMode.Create);
        stream.Write(data);
        stream.Flush();
    }

    /// <inheritdoc />
    public void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            logger.Debug($"Creating directory: {path}");
            Directory.CreateDirectory(path);                
        }
    }

    /// <inheritdoc />
    public IEnumerable<string> GetFileNames(string? path, string pattern, bool recursive)
    {
        if (string.IsNullOrWhiteSpace(path))
            path = ".\\";

        logger.Debug(string.IsNullOrWhiteSpace(pattern)
            ? $"Getting all files from path: {path} (no pattern)"
            : $"Getting all files from path: {path} (with pattern {pattern})");

        var searchOption = recursive
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;

        var result = string.IsNullOrWhiteSpace(pattern)
            ? Directory.GetFiles(path, "*", searchOption)
            : Directory.GetFiles(path, pattern, searchOption);
            
        return result;
    }

    /// <inheritdoc />
    public IEnumerable<string> GetDirectoryNames(string path)
    {
        logger.Debug($"Getting all directories from path: {path}");
        return Directory
            .GetDirectories(path)
            .Select(Path.GetFileName)
            .Where(f => f != null)
            .Select(f => f!);
    }

    /// <inheritdoc />
    public string? GetDirectory(string path)
    {
        return Path.GetDirectoryName(path);
    }

    /// <inheritdoc />
    public string GetSafeFileName(string fileName)
    {
        return new string(fileName.Select(c => _invalidChars.Contains(c) ? SafeChar : c).ToArray());
    }
}