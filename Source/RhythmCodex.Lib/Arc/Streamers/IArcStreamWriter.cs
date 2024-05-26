using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using RhythmCodex.Arc.Model;

namespace RhythmCodex.Arc.Streamers;

/// <summary>
/// Writes files to an ARC archive.
/// </summary>
[PublicAPI]
public interface IArcStreamWriter
{
    /// <summary>
    /// Write files to an ARC archive.
    /// </summary>
    void Write(Stream target, IEnumerable<ArcFile> files);
}