using RhythmCodex.Graphics.Models;
using RhythmCodex.Tga.Models;

namespace RhythmCodex.Tga.Converters;

public interface ITgaDecoder
{
    bool IsIndexedPalette(TgaImage tgaImage);
    Bitmap Decode(TgaImage tgaImage);
    PaletteBitmap DecodeIndexed(TgaImage tgaImage);
}