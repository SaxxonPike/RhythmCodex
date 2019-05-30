using System.Drawing;
using System.IO;
using RhythmCodex.Gdi.Converters;
using RhythmCodex.Graphics.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Gdi.Streamers
{
    [Service]
    public class BitmapStreamReader : IBitmapStreamReader
    {
        private readonly IGdiFactory _gdiFactory;

        public BitmapStreamReader(IGdiFactory gdiFactory)
        {
            _gdiFactory = gdiFactory;
        }
        
        public RawBitmap Read(Stream stream)
        {
            using (var source = new Bitmap(stream))
            {
                var result = new RawBitmap
                {
                    Width = source.Width,
                    Height = source.Height,
                    Data = new int[source.Width * source.Height]
                };
                
                using (var adapter = _gdiFactory.CreateAdapter(result))
                using (var g = _gdiFactory.CreateGraphics(adapter.Bitmap))
                {
                    g.DrawImage(source, 0, 0);
                }

                return result;
            }
        }
    }
}