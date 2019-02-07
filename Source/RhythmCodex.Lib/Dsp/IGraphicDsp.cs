using System.Drawing;
using RhythmCodex.Graphics.Models;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Dsp
{
    public interface IGraphicDsp
    {
        RawBitmap Snip(RawBitmap bitmap, Rectangle rect);
    }
}