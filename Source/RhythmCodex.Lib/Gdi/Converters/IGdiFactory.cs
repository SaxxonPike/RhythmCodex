using System.Drawing;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Gdi.Converters
{
    public interface IGdiFactory
    {
        IGdiAdapter CreateAdapter(RawBitmap bitmap);
        Bitmap CreateBitmap(int width, int height);
        Graphics CreateGraphics(Image image);
    }
}