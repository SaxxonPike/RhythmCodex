using System.Collections.Generic;
using RhythmCodex.Graphics.Models;
using RhythmCodex.Graphics.Tim.Models;

namespace RhythmCodex.Graphics.Tim.Converters;

public interface ITimBitmapDecoder
{
    List<Bitmap> Decode(TimImage image);
    Bitmap Decode(TimImage image, int clutIndex);
}