using RhythmCodex.Infrastructure;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Graphics.Models;

[Model]
public class PaletteBitmap : Bitmap
{
    public PaletteBitmap(int width, int[] data, int[] palette) : base(width, data)
    {
        Palette = palette;
    }

    public PaletteBitmap(int width, int height, int colors) : base(width, height)
    {
        Palette = new int[colors];
    }

    public int[] Palette { get; }
}