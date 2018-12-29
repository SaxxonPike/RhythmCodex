using System.Drawing;
using System.Drawing.Imaging;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Gdi.Converters
{
    [Service]
    public class GdiFactory : IGdiFactory
    {
        public IGdiAdapter CreateAdapter(RawBitmap bitmap) => 
            new GdiAdapter(bitmap.Data, bitmap.Width, bitmap.Height, bitmap.Width * 4);

        public Bitmap CreateBitmap(int width, int height) =>
            new Bitmap(width, height, PixelFormat.Format32bppArgb);

        public Graphics CreateGraphics(Image image) =>
            Graphics.FromImage(image);
    }
}