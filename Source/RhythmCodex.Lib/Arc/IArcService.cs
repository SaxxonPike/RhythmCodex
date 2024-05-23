using System.Collections.Generic;
using System.IO;
using RhythmCodex.Arc.Model;

namespace RhythmCodex.Arc;

/// <summary>
/// Handles ARC archives.
/// </summary>
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