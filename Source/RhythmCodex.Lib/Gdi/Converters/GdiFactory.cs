using System.Drawing;
using System.Drawing.Imaging;
using RhythmCodex.Graphics.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Gdi.Converters
{
    [Service]
    public class GdiFactory : IGdiFactory
    {
        public IGdiAdapter CreateAdapter(RawBitmap bitmap) => 
            new GdiAdapter(bitmap.Data, bitmap.Width, bitmap.Height, bitmap.Width * 4);

        public Bitmap CreateBitmap(int width, int height) =>
            new Bitmap(width, height, PixelFormat.Format32bppArgb);

        public System.Drawing.Graphics CreateGraphics(Image image) =>
            System.Drawing.Graphics.FromImage(image);
    }
}