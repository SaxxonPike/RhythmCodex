using RhythmCodex.Graphics.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RhythmCodex.Plugin.ImageSharp;

public interface IImageSharpBitmapConverter
{
    Image<Rgba32> ConvertBitmap(Bitmap bitmap);
    Bitmap ConvertImage(Image<Rgba32> image);
}