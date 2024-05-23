using RhythmCodex.Graphics.Models;
using RhythmCodex.IoC;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RhythmCodex.Plugin.ImageSharp;

[Service]
public class ImageSharpBitmapConverter : IImageSharpBitmapConverter
{
    public Image<Rgba32> ConvertBitmap(IBitmap bitmap)
    {
        var image = new Image<Rgba32>(bitmap.Width, bitmap.Height);
        image.ProcessPixelRows(accessor =>
        {
            var i = 0;
            for (var y = 0; y < accessor.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                for (var x = 0; x < accessor.Width; x++, i++)
                {
                    var pixel = bitmap.Data[i];
                    row[x] = new Rgba32((byte)pixel,
                        (byte)(pixel >> 8),
                        (byte)(pixel >> 16),
                        (byte)(pixel >> 24));
                }
            }
        });
        return image;
    }

    public IBitmap ConvertImage(Image<Rgba32> image)
    {
        var bitmap = new Bitmap(image.Width, image.Height);
        image.ProcessPixelRows(accessor =>
        {
            var i = 0;
            for (var y = 0; y < accessor.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                for (var x = 0; x < accessor.Width; x++, i++)
                {
                    var pixel = row[x];
                    bitmap.Data[i] = pixel.R |
                                     (pixel.G << 8) |
                                     (pixel.B << 16) |
                                     (pixel.A << 24);
                }
            }
        });

        return bitmap;
    }
}