using RhythmCodex.Gdi.Streamers;
using RhythmCodex.Graphics.Models;
using RhythmCodex.IoC;
using SixLabors.ImageSharp;

namespace RhythmCodex.Plugin.ImageSharp;

[Service]
public class ImageSharpPngStreamWriter : IPngStreamWriter
{
    private readonly IImageSharpBitmapConverter _imageSharpBitmapConverter;

    public ImageSharpPngStreamWriter(IImageSharpBitmapConverter imageSharpBitmapConverter)
    {
        _imageSharpBitmapConverter = imageSharpBitmapConverter;
    }
    
    public void Write(Stream stream, IBitmap bitmap)
    {
        using var image = _imageSharpBitmapConverter.ConvertBitmap(bitmap);
        image.SaveAsPng(stream);
    }
}