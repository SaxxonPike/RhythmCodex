using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using RhythmCodex.FileSystems.Arc.Model;

namespace RhythmCodex.FileSystems.Arc.Streamers;

/// <summary>
/// Reads files from an ARC archive.
/// </summary>
[PublicAPI]
public interface IArcStreamReader
{
    /// <summary>
    /// Read files from an ARC archive.
    /// </summary>
    IEnumerable<ArcFile> Read(Stream source);
}