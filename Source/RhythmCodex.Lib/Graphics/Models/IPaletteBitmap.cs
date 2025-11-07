using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Graphics.Models;

public interface IPaletteBitmap : IMetadata
{
    int Width { get; }
    int Height { get; }
    int[] Data { get; }
    int[] Palette { get; }
}