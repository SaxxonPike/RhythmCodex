using RhythmCodex.Graphics.Models;
using RhythmCodex.Tga.Models;

namespace RhythmCodex.Tga.Converters;

public interface ITgaDecoder
{
    bool IsIndexedPalette(TgaImage tgaImage);
    IBitmap Decode(TgaImage tgaImage);
    IPaletteBitmap DecodeIndexed(TgaImage tgaImage);
}