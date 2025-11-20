using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Graphics.Models;
using RhythmCodex.Graphics.Tim.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Graphics.Tim.Converters;

[Service]
public class TimBitmapDecoder(ITimColorDecoder colorDecoder, ITimDataDecoder dataDecoder) : ITimBitmapDecoder
{
    public List<Bitmap> Decode(TimImage image) =>
        Enumerable.Range(0, image.Cluts.Count).Select(i => Decode(image, i)).ToList();

    public Bitmap Decode(TimImage image, int clutIndex)
    {
        var data = DecodeData(image);
        ConvertColors(data, image, clutIndex);
            
        return new Bitmap(data.Length / image.Height, data);
    }

    private void ConvertColors(Span<int> data, TimImage image, int clutIndex)
    {
        switch (image.ImageType & 0x3)
        {
            case 0x00000000:
            case 0x00000001:
                var clut = GetClut(image, clutIndex);
                for (var i = 0; i < data.Length; i++)
                    data[i] = clut![data[i]];
                break;
            case 0x00000002:
                for (var i = 0; i < data.Length; i++)
                    data[i] = colorDecoder.Decode16Bit(data[i]);
                break;
            case 0x00000003:
                for (var i = 0; i < data.Length; i++)
                    data[i] = colorDecoder.Decode24Bit(data[i]);
                break;
        }
    }

    private int[]? GetClut(TimImage image, int clutIndex)
    {
        switch (image.ImageType & 0x3)
        {
            case 0x00000000:
            case 0x00000001:
                return DecodeClut(image.Cluts[clutIndex]);
            default:
                return null;
        }            
    }

    private int[] DecodeClut(TimPalette clut) => 
        clut.Entries.Select(i => colorDecoder.Decode16Bit(i)).ToArray();

    private int[] DecodeData(TimImage image)
    {
        switch (image.ImageType & 0x3)
        {
            case 0x00000000:
                return dataDecoder.Decode4Bit(image.Data.Span, image.Stride * 2, image.Height);
            case 0x00000001:
                return dataDecoder.Decode8Bit(image.Data.Span, image.Stride * 2, image.Height);
            case 0x00000002:
                return dataDecoder.Decode16Bit(image.Data.Span, image.Stride * 2, image.Height);
            case 0x00000003:
                return dataDecoder.Decode24Bit(image.Data.Span, image.Stride * 2, image.Height);
            default:
                throw new RhythmCodexException($"Unrecognized TIM image type {image.ImageType:X8}.");
        }
    }
}