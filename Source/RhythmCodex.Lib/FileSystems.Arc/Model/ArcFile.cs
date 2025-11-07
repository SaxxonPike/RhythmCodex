using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.FileSystems.Arc.Model;

/// <summary>
/// Represents a single file within an ARC archive.
/// </summary>
[Model]
public class ArcFile
{
    /// <summary>
    /// File name of the file.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// File size, compressed.
    /// </summary>
    public int CompressedSize { get; set; }
    
    /// <summary>
    /// File size, not compressed.
    /// </summary>
    public int DecompressedSize { get; set; }
    
    /// <summary>
    /// Raw data. Can be compressed or uncompressed. If this data is compressed, there will be a difference
    /// between <see cref="CompressedSize"/> and <see cref="DecompressedSize"/>.
    /// </summary>
    public Memory<byte> Data { get; set; }
}