using RhythmCodex.Gdi.Streamers;
using RhythmCodex.Graphics.Models;
using RhythmCodex.IoC;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RhythmCodex.Plugin.ImageSharp;

[Service]
public class ImageSharpBitmapStreamReader : IBitmapStreamReader
{
    private readonly IImageSharpBitmapConverter _imageSharpBitmapConverter;

    public ImageSharpBitmapStreamReader(IImageSharpBitmapConverter imageSharpBitmapConverter)
    {
        _imageSharpBitmapConverter = imageSharpBitmapConverter;
    }

    public IBitmap Read(Stream stream)
    {
        using var image = Image.Load<Rgba32>(stream);
        return _imageSharpBitmapConverter.ConvertImage(image);
    }
}