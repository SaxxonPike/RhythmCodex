using RhythmCodex.Dds.Models;
using RhythmCodex.Graphics.Models;

namespace RhythmCodex.Dds.Converters
{
    public interface IDdsBitmapDecoder
    {
        RawBitmap Decode(DdsImage image);
    }
}