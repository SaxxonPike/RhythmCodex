using RhythmCodex.Graphics.Converters;
using RhythmCodex.Graphics.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Tga.Models;

namespace RhythmCodex.Tga.Converters;

[Service]
public class TgaDecoder(IGraphicDsp graphicDsp) : ITgaDecoder
{
    public bool IsIndexedPalette(TgaImage tgaImage)
    {
        switch (tgaImage.DataTypeCode)
        {
            case TgaDataType.RleIndexed:
            case TgaDataType.HuffmanIndexed:
            case TgaDataType.UncompressedIndexed:
            case TgaDataType.HuffmanQuadtreeIndexed:
                return true;
            default:
                return false;
        }
    }

    public Bitmap Decode(TgaImage tgaImage)
    {
        if (tgaImage.Interleave != TgaInterleave.None)
            throw new RhythmCodexException("Only non-interleaved images are supported for now.");

        if (IsIndexedPalette(tgaImage))
            return graphicDsp.DeIndex(DecodeIndexed(tgaImage));

        switch (tgaImage.DataTypeCode)
        {
            case TgaDataType.UncompressedRgb:
            {
                var count = tgaImage.Width * tgaImage.Height;
                var pixels = new int[count];
                var inputIndex = 0;
                var outputIndex = tgaImage.OriginType == TgaOriginType.UpperLeft
                    ? 0
                    : tgaImage.Width * (tgaImage.Height - 1);
                var scanIncrement = tgaImage.OriginType == TgaOriginType.UpperLeft
                    ? 0
                    : tgaImage.Width * -2;

                var data = tgaImage.ImageData.Span;

                switch (tgaImage.BitsPerPixel)
                {
                    case 24:
                    {
                        for (var y = 0; y < tgaImage.Height; y++)
                        {
                            for (var x = 0; x < tgaImage.Width; x++)
                            {
                                pixels[outputIndex++] = ~0x00FFFFFF |
                                                        data[inputIndex] |
                                                        (data[inputIndex + 1] << 8) |
                                                        (data[inputIndex + 2] << 16);
                                inputIndex += 3;
                            }

                            outputIndex += scanIncrement;
                        }

                        return new Bitmap(tgaImage.Width, pixels);
                    }
                    case 32:
                    {
                        for (var y = 0; y < tgaImage.Height; y++)
                        {
                            for (var x = 0; x < tgaImage.Width; x++)
                            {
                                pixels[outputIndex++] = data[inputIndex] |
                                                        (data[inputIndex + 1] << 8) |
                                                        (data[inputIndex + 2] << 16) |
                                                        (data[inputIndex + 3] << 24);
                                inputIndex += 4;
                            }

                            outputIndex += scanIncrement;
                        }

                        return new Bitmap(tgaImage.Width, pixels);
                    }
                    default:
                    {
                        throw new RhythmCodexException($"Bit depth is not supported: {tgaImage.BitsPerPixel}");
                    }
                }
            }
            default:
            {
                throw new RhythmCodexException($"This TGA image type is not supported: {tgaImage.DataTypeCode}");
            }
        }
    }

    public PaletteBitmap DecodeIndexed(TgaImage tgaImage)
    {
        if (!IsIndexedPalette(tgaImage))
            throw new RhythmCodexException(
                $"{nameof(DecodeIndexed)} can only be used with images that contain a palette.");

        var palette = new int[tgaImage.ColorMapLength];
        var paletteSize = tgaImage.ColorMapLength * tgaImage.ColorMapBitsPerEntry / 8;
        var paletteData = tgaImage.ImageData.Span[0..paletteSize];

        switch (tgaImage.ColorMapBitsPerEntry)
        {
            case 16:
            {
                for (var i = 0; i < tgaImage.ColorMapLength; i++)
                {
                    var sourceIndex = i << 1;
                    var entry = paletteData[sourceIndex] |
                                (paletteData[sourceIndex + 1] << 8);
                    var blue = (entry & 0x1F) << 3;
                    var green = (entry >> 2) & 0xF8;
                    var red = (entry >> 7) & 0xF8;
                    var alpha = (entry >> 15) * 0xFF;
                    palette[i] = blue | (green << 8) | (red << 16) | (alpha << 24);
                }

                break;
            }
            case 24:
            {
                var inputIndex = 0;
                for (var i = 0; i < tgaImage.ColorMapLength; i++)
                {
                    palette[i] = ~0x00FFFFFF |
                                 paletteData[inputIndex++] |
                                 (paletteData[inputIndex++] << 8) |
                                 (paletteData[inputIndex++] << 16);
                }

                break;
            }
            case 32:
            {
                var inputIndex = 0;
                for (var i = 0; i < tgaImage.ColorMapLength; i++)
                {
                    palette[i] = paletteData[inputIndex++] |
                                 (paletteData[inputIndex++] << 8) |
                                 (paletteData[inputIndex++] << 16) |
                                 (paletteData[inputIndex++] << 24);
                }

                break;
            }
            default:
            {
                throw new RhythmCodexException("Only palettes with color index sizes 2-4 are supported.");
            }
        }

        switch (tgaImage.DataTypeCode)
        {
            case TgaDataType.UncompressedIndexed:
            {
                if (tgaImage.BitsPerPixel != 8)
                    throw new RhythmCodexException("Only 8bpp is supported for indexed images for now.");

                var count = tgaImage.Width * tgaImage.Height;
                var pixels = new int[count];
                var inputIndex = 0;
                var outputIndex = tgaImage.OriginType == TgaOriginType.UpperLeft
                    ? 0
                    : tgaImage.Width * (tgaImage.Height - 1);
                var scanIncrement = tgaImage.OriginType == TgaOriginType.UpperLeft
                    ? 0
                    : tgaImage.Width * -2;

                var data = tgaImage.ImageData.Span[paletteSize..];

                for (var y = 0; y < tgaImage.Height; y++)
                {
                    for (var x = 0; x < tgaImage.Width; x++)
                        pixels[outputIndex++] = data[inputIndex++];
                    outputIndex += scanIncrement;
                }

                return new PaletteBitmap
                {
                    Width = tgaImage.Width,
                    Height = tgaImage.Height,
                    Data = pixels,
                    Palette = palette
                };
            }
            default:
            {
                throw new RhythmCodexException($"This TGA image type is not supported: {tgaImage.DataTypeCode}");
            }
        }
    }
}