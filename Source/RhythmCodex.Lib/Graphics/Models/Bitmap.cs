using RhythmCodex.Infrastructure;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Graphics.Models;

[Model]
public class Bitmap : Metadata
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
        
    public int Width { get;  }
    public int Height { get;  }
    public int[] Data { get; }
}