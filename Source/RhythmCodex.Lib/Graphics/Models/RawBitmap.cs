using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Graphics.Models
{
    [Model]
    public class RawBitmap : Metadata, IRawBitmap
    {
        public RawBitmap()
        {
        }

        public RawBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Data = new int[width * height];
        }
        
        public int Width { get; set; }
        public int Height { get; set; }
        public int[] Data { get; set; }
    }
}