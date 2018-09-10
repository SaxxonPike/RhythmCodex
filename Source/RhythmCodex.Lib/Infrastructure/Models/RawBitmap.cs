namespace RhythmCodex.Infrastructure.Models
{
    [Model]
    public class RawBitmap
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int[] Data { get; set; }
    }
}