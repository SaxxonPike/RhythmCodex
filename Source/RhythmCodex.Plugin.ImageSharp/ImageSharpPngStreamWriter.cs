using RhythmCodex.Gdi.Streamers;
using RhythmCodex.Graphics.Models;
using RhythmCodex.IoC;
using SixLabors.ImageSharp;

namespace RhythmCodex.Plugin.ImageSharp;

[Service]
public class ImageSharpPngStreamWriter(IImageSharpBitmapConverter imageSharpBitmapConverter) : IPngStreamWriter
{
    public void Write(Stream stream, IBitmap bitmap)
    {
        using var image = imageSharpBitmapConverter.ConvertBitmap(bitmap);
        image.SaveAsPng(stream);
    }
}