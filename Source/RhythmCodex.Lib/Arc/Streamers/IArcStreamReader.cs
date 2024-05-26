using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using RhythmCodex.Arc.Model;

namespace RhythmCodex.Arc.Streamers;

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