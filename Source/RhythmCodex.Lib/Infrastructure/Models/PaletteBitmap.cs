using System.Linq;

namespace RhythmCodex.Infrastructure.Models
{
    [Model]
    public class PaletteBitmap
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int[] Data { get; set; }
        public int[] Palette { get; set; }

        public RawBitmap ToRawBitmap()
        {
            return new RawBitmap
            {
                Width = Width,
                Height = Height,
                Data = Data.Select(d => Palette[d]).ToArray()
            };
        }
    }
}