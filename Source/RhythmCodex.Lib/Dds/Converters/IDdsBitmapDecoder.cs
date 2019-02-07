using RhythmCodex.Dds.Models;
using RhythmCodex.Graphics.Models;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Dds.Converters
{
    public interface IDdsBitmapDecoder
    {
        RawBitmap Decode(DdsImage image);
    }
}