using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using RhythmCodex.FileSystems.Arc.Model;

namespace RhythmCodex.FileSystems.Arc;

/// <summary>
/// Handles ARC archives.
/// </summary>
[PublicAPI]
public interface IArcService
{
    /// <summary>
    /// Read an ARC archive.
    /// </summary>
    List<ArcFile> ReadArc(Stream stream);

    /// <summary>
    /// Write an ARC archive.
    /// </summary>
    void WriteArc(Stream stream, IEnumerable<ArcFile> files);
}