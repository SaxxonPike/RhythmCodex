using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Graphics.Models;

[Model]
public class Bitmap : Metadata, IBitmap
{
    public Bitmap(int width, int[] data)
    {
        if (width == 0)
        {
            Width = 0;
            Data = [];
        }
        else
        {
            Width = width;
            Height = data.Length / Width;
            Data = data;
        }
    }

    public Bitmap(int width, int height)
    {
        Width = width;
        Height = height;
        Data = new int[width * height];
    }
        
    public int Width { get; init; }
    public int Height { get; init; }
    public int[] Data { get; init; }
}