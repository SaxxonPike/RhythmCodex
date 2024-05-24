using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Tim.Models;

namespace RhythmCodex.Tim.Streamers;

[Service]
public class TimStreamReader : ITimStreamReader
{
    public TimImage Read(Stream stream)
    {
        var reader = new BinaryReader(stream);
        if (reader.ReadInt32() != 0x00000010)
            throw new RhythmCodexException("Unrecognized TIM image identifier.");

        var cluts = new List<TimPalette>();
        var result = new TimImage
        {
            ImageType = reader.ReadInt32(),
            Cluts = cluts
        };

        var hasCluts = (result.ImageType & 0x8) != 0;
        if (hasCluts)
            cluts.AddRange(ReadCluts(stream));                

        var bpp = GetBpp(result.ImageType);
        var length = reader.ReadInt32();
        result.OriginX = reader.ReadInt16();
        result.OriginY = reader.ReadInt16();
        result.Stride = reader.ReadInt16();
        result.Height = reader.ReadInt16();
        var padding = length - result.Stride * result.Height * 8 / bpp - 12;
        result.Data = reader.ReadBytes(length);

        if (padding > 0)
            reader.ReadBytes(padding);

        return result;
    }

    private int GetBpp(int resultImageType)
    {
        switch (resultImageType & 0x03)
        {
            case 0: return 4;
            case 1: return 8;
            case 2: return 16;
            default: return 24;
        }
    }

    private IEnumerable<TimPalette> ReadCluts(Stream stream)
    {
        var reader = new BinaryReader(stream);

        var length = reader.ReadInt32();
        var originX = reader.ReadInt16();
        var originY = reader.ReadInt16();
        int numColors = reader.ReadInt16();
        int numCluts = reader.ReadInt16();
        var padding = length - numCluts * numColors * 2 - 12;

        for (var i = 0; i < numCluts; i++)
        {
            var entries = new List<short>(numColors);
            var clut = new TimPalette
            {
                OriginX = originX,
                OriginY = originY,
                Entries = entries
            };
                
            for (var j = 0; j < numColors; j++)
                entries.Add(reader.ReadInt16());

            yield return clut;
        }

        if (padding > 0)
            reader.ReadBytes(padding);
    }
}