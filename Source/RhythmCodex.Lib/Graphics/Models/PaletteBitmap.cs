using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Graphics.Models
{
    [Model]
    public class PaletteBitmap : Metadata, IPaletteBitmap
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