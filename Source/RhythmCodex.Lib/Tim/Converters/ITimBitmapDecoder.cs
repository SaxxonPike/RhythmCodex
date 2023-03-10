using System.Collections.Generic;
using RhythmCodex.Graphics.Models;
using RhythmCodex.Tim.Models;

namespace RhythmCodex.Tim.Converters
{
    public interface ITimBitmapDecoder
    {
        IList<IBitmap> Decode(TimImage image);
        IBitmap Decode(TimImage image, int clutIndex);
    }
}