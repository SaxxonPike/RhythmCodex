using System.Collections.Generic;
using RhythmCodex.Graphics.Models;
using RhythmCodex.Tim.Models;

namespace RhythmCodex.Tim.Converters;

public interface ITimBitmapDecoder
{
    List<Bitmap> Decode(TimImage image);
    Bitmap Decode(TimImage image, int clutIndex);
}