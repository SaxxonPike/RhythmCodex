namespace RhythmCodex.Infrastructure.Models
{
    [Model]
    public class RawBitmap
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