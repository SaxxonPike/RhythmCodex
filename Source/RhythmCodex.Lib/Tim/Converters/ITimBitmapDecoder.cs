using System.Collections.Generic;
using RhythmCodex.Graphics.Models;
using RhythmCodex.Tim.Models;

namespace RhythmCodex.Tim.Converters
{
    public interface ITimBitmapDecoder
    {
        IList<RawBitmap> Decode(TimImage image);
        RawBitmap Decode(TimImage image, int clutIndex);
    }
}