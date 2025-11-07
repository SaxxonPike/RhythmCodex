using RhythmCodex.Graphics.Dds.Models;
using RhythmCodex.Graphics.Models;

namespace RhythmCodex.Graphics.Dds.Converters;

public interface IDdsBitmapDecoder
{
    Bitmap Decode(DdsImage image);
}