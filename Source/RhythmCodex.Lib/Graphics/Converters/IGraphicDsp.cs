using System.Drawing;
using RhythmCodex.Graphics.Models;

namespace RhythmCodex.Graphics.Converters
{
    public interface IGraphicDsp
    {
        RawBitmap Snip(RawBitmap bitmap, Rectangle rect);
    }
}