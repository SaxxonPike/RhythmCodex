using RhythmCodex.Graphics.Models;
using RhythmCodex.Tga.Models;

namespace RhythmCodex.Tga.Converters
{
    public interface ITgaDecoder
    {
        bool IsIndexedPalette(TgaImage tgaImage);
        RawBitmap Decode(TgaImage tgaImage);
        PaletteBitmap DecodeIndexed(TgaImage tgaImage);
    }
}