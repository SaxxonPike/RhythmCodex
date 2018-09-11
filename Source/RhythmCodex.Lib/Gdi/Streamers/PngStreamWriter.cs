using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using RhythmCodex.Gdi.Converters;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Gdi.Streamers
{
    public class PngStreamWriter : IPngStreamWriter
    {
        private readonly IGdiFactory _gdiFactory;

        public PngStreamWriter(IGdiFactory gdiFactory)
        {
            _gdiFactory = gdiFactory;
        }
        
        public void Write(Stream stream, RawBitmap rawBitmap)
        {
            using (var adapter = _gdiFactory.CreateAdapter(rawBitmap))
            using (var output = _gdiFactory.CreateBitmap(rawBitmap.Width, rawBitmap.Height))
            using (var g = _gdiFactory.CreateGraphics(output))
            {
                g.DrawImage(adapter.Bitmap, 0, 0);
                output.Save(stream, ImageFormat.Png);
            }
        }
    }
}