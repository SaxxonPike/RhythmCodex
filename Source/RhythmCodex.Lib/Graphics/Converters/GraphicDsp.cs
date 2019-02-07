using System.Drawing;
using RhythmCodex.Dsp;
using RhythmCodex.Gdi.Converters;
using RhythmCodex.Graphics.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Graphics.Converters
{
    [Service]
    public class GraphicDsp : IGraphicDsp
    {
        private readonly IGdiFactory _gdiFactory;

        public GraphicDsp(IGdiFactory gdiFactory)
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