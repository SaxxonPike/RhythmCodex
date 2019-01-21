using System.Drawing;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Gdi.Converters
{
    [Service]
    public class GdiDsp : IGdiDsp
    {
        private readonly IGdiFactory _gdiFactory;

        public GdiDsp(IGdiFactory gdiFactory)
        {
            _gdiFactory = gdiFactory;
        }

        public RawBitmap Snip(RawBitmap bitmap, Rectangle rect)
        {
            var target = new RawBitmap(rect.Width, rect.Height);
            using (var sourceAdapter = _gdiFactory.CreateAdapter(bitmap))
            using (var targetAdapter = _gdiFactory.CreateAdapter(target))
            using (var targetGraphics = _gdiFactory.CreateGraphics(targetAdapter.Bitmap))
            {
                targetGraphics.DrawImage(
                    sourceAdapter.Bitmap,
                    new Rectangle(0, 0, rect.Width, rect.Height),
                    rect,
                    GraphicsUnit.Pixel);
            }

            return target;
        }
    }
}