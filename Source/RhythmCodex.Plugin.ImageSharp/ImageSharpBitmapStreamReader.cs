using RhythmCodex.Graphics.Gdi.Streamers;
using RhythmCodex.Graphics.Models;
using RhythmCodex.IoC;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RhythmCodex.Plugin.ImageSharp;

[Service]
public class ImageSharpBitmapStreamReader(IImageSharpBitmapConverter imageSharpBitmapConverter) : IBitmapStreamReader
{
    public Bitmap Read(Stream stream)
    {
        using var image = Image.Load<Rgba32>(stream);
        return imageSharpBitmapConverter.ConvertImage(image);
    }
}