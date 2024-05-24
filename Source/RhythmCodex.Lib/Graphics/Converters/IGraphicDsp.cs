using System.Drawing;
using RhythmCodex.Graphics.Models;

namespace RhythmCodex.Graphics.Converters;

public interface IGraphicDsp
{
    Bitmap DeIndex(PaletteBitmap bitmap);
    Bitmap Snip(Bitmap bitmap, Rectangle rect);
}