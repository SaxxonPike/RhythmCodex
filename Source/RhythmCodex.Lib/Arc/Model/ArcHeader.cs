using RhythmCodex.Infrastructure;

namespace RhythmCodex.Arc.Model;

/// <summary>
/// Represents raw header data in an ARC archive.
/// </summary>
[Model]
public class ArcHeader
{
    /// <summary>
    /// Archive ID.
    /// </summary>
    public int Id { get; set; }
    
    public int Unk0 { get; set; }
    
    /// <summary>
    /// Number of files in the archive.
    /// </summary>
    public int FileCount { get; set; }
    
    public int Unk1 { get; set; }
}