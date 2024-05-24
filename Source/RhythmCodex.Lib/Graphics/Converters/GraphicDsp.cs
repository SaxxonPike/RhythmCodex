using System;
using System.Drawing;
using RhythmCodex.Graphics.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Graphics.Converters;

[Service]
public class GraphicDsp : IGraphicDsp
{
    public Bitmap DeIndex(PaletteBitmap bitmap)
    {
        var palette = bitmap.Palette;
        var sourceData = bitmap.Data;
        var targetData = new int[bitmap.Width * bitmap.Height];
        var length = sourceData.Length;

        for (var i = 0; i < length; i++)
            targetData[i] = palette[sourceData[i]];

        return new Bitmap(bitmap.Width, targetData);
    }

    public Bitmap Snip(Bitmap bitmap, Rectangle rect)
    {
        var sourceData = bitmap.Data.AsSpan();
        var sourceOffset = rect.Top * bitmap.Width + rect.Left;
        var sourceStride = bitmap.Width;
        var target = new Bitmap(rect.Width, rect.Height);
        var targetData = target.Data.AsSpan();
        var targetOffset = 0;
        var targetStride = rect.Width;

        for (var y = 0; y < rect.Height; y++)
        {
            sourceData.Slice(sourceOffset, targetStride).CopyTo(targetData[targetOffset..]);
            sourceOffset += sourceStride;
            targetOffset += targetStride;
        }
            
        return target;
    }
}