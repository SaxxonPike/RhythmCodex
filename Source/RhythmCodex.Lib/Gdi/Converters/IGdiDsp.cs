using System.Drawing;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Gdi.Converters
{
    public interface IGdiDsp
    {
        RawBitmap Snip(RawBitmap bitmap, Rectangle rect);
    }
}