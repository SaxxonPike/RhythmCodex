using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

namespace ClientCommon;

[PublicAPI]
public interface IFileSystem
{
    /// <summary>
    ///     Get the current working path.
    /// </summary>
    string CurrentPath { get; }

    /// <summary>
    ///     Get the file name portion of a path.
    /// </summary>
    string GetFileName(string path);

    /// <summary>
    ///     Open a file stream for reading.
    /// </summary>
    Stream OpenRead(string path);

    /// <summary>
    ///     Open a file stream for writing.
    /// </summary>
    Stream OpenWrite(string path);

    /// <summary>
    ///     Combine multiple paths.
    /// </summary>
    string CombinePath(params string[] paths);

    /// <summary>
    ///     Read all bytes from the specified file path.
    /// </summary>
    Memory<byte> ReadAllBytes(string path);

    /// <summary>
    ///     Write all bytes to the specified file path.
    /// </summary>
    void WriteAllBytes(string path, ReadOnlySpan<byte> data);

    /// <summary>
    ///     Create a directory at the specified path.
    /// </summary>
    void CreateDirectory(string path);

    /// <summary>
    ///     Get all file names from the specified directory path.
    /// </summary>
    IEnumerable<string> GetFileNames(string? path, string pattern, bool recursive = false);

    /// <summary>
    ///     Get all subdirectory names from the specified directory path.
    /// </summary>
    IEnumerable<string> GetDirectoryNames(string path);

    /// <summary>
    ///     Get the directory portion of a path.
    /// </summary>
    string? GetDirectory(string path);

    /// <summary>
    ///     Replace all invalid path characters with a safe alternative character.
    /// </summary>
    string GetSafeFileName(string fileName);
}