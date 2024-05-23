using System.Collections.Generic;
using System.IO;
using RhythmCodex.Arc.Model;

namespace RhythmCodex.Arc.Streamers;

/// <summary>
/// Writes files to an ARC archive.
/// </summary>
public interface IArcStreamWriter
{
    /// <summary>
    /// Write files to an ARC archive.
    /// </summary>
    void Write(Stream target, IEnumerable<ArcFile> files);
}