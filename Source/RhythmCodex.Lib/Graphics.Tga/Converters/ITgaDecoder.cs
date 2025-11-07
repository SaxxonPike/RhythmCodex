using RhythmCodex.Graphics.Models;
using RhythmCodex.Graphics.Tga.Models;

namespace RhythmCodex.Graphics.Tga.Converters;

public interface ITgaDecoder
{
    bool IsIndexedPalette(TgaImage tgaImage);
    Bitmap Decode(TgaImage tgaImage);
    PaletteBitmap DecodeIndexed(TgaImage tgaImage);
}