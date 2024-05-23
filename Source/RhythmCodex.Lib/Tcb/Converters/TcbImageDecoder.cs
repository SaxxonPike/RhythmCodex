using System;
using RhythmCodex.Graphics.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Tcb.Models;

namespace RhythmCodex.Tcb.Converters;

[Service]
public class TcbImageDecoder : ITcbImageDecoder
{
    public IBitmap Decode(TcbImage image)
    {
        var result = new Bitmap(image.Width, image.Height);
        var resultData = result.Data;
        var imageData = image.Image;
        var pixelCount = image.Width * image.Height;
        int[] palette = null;

        //if (image.Palette.Length > 0x50)
        {
            var palSpan = Bitter.ToInt32Values(image.Palette);
            palette = FilterPalette(palSpan);
        }

        int ConvertPixel(int px)
        {
            // fill the upper 8 bits with the alpha bit with this one cheap hack
            px = ((px >> 7) & ~0xFFFFFF) | (px & 0xFFFFFF);
            // swap red and blue
            px = (px & ~0xFF00FF) | ((px & 0xFF) << 16) | ((px & 0xFF0000) >> 16);
            return px;
        }

        switch (image.PaletteType)
        {
            case 0x05:
            {
                for (var i = 0; i < pixelCount; i++)
                {
                    var pixel = palette[imageData[i]];
                    resultData[i] = ConvertPixel(pixel);
                }
                break;
            }
            case 0x04:
            {
                var imageIndex = 0;
                for (var i = 0; i < pixelCount / 2; i++)
                {
                    var image0 = imageData[i] & 0xF;
                    var image1 = imageData[i] >> 4;
                        
                    var pixel = palette[image0];
                    resultData[imageIndex++] = ConvertPixel(pixel);
                    pixel = palette[image1];
                    resultData[imageIndex++] = ConvertPixel(pixel);
                }
                break;
            }
            default:
            {
                return null;
            }
        }

        return result;
    }

    private int[] FilterPalette(ReadOnlySpan<int> palette)
    {
        var length = palette.Length;
        var result = new int[palette.Length];
        var parts = length / 32;
        const int stripes = 2;
        const int colors = 8;
        const int blocks = 2;

        var i = 0;

        for (var part = 0; part < parts; part++)
        for (var block = 0; block < blocks; block++)
        for (var stripe = 0; stripe < stripes; stripe++)
        for (var color = 0; color < colors; color++)
        {
            result[i++] = palette[
                part * colors * stripes * blocks + block * colors + stripe * stripes * colors + color];
        }

        return result;
    }
}