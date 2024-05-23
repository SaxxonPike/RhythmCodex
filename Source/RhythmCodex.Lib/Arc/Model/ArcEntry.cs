using RhythmCodex.Infrastructure;

namespace RhythmCodex.Arc.Model;

/// <summary>
/// Represents a single file definition within an ARC archive.
/// </summary>
[Model]
public class ArcEntry
{
    /// <summary>
    /// Offset in the archive where the file name is present.
    /// </summary>
    public int NameOffset { get; set; }

    /// <summary>
    /// Offset in the archive where the file data is present.
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// Number of bytes in the file data when decompressed.
    /// </summary>
    public int DecompressedSize { get; set; }

    /// <summary>
    /// Number of bytes in the file data when compressed.
    /// </summary>
    public int CompressedSize { get; set; }
}