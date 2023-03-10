using System.Drawing;
using RhythmCodex.Graphics.Models;

namespace RhythmCodex.Graphics.Converters
{
    public interface IGraphicDsp
    {
        IBitmap DeIndex(IPaletteBitmap bitmap);
        IBitmap Snip(IBitmap bitmap, Rectangle rect);
    }
}