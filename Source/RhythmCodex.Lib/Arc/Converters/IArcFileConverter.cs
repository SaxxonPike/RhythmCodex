using RhythmCodex.Arc.Model;

namespace RhythmCodex.Arc.Converters;

/// <summary>
/// Compresses and decompresses ARC archive file entries.
/// </summary>
public interface IArcFileConverter
{
    /// <summary>
    /// Compress ARC archive file data.
    /// </summary>
    ArcFile Compress(ArcFile file);
    
    /// <summary>
    /// Decompress ARC archive file data.
    /// </summary>
    ArcFile Decompress(ArcFile file);
}