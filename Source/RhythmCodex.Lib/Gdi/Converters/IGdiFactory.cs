using System.Drawing;
using RhythmCodex.Graphics.Models;

namespace RhythmCodex.Gdi.Converters
{
    public interface IGdiFactory
    {
        IGdiAdapter CreateAdapter(RawBitmap bitmap);
        Bitmap CreateBitmap(int width, int height);
        System.Drawing.Graphics CreateGraphics(Image image);
    }
}