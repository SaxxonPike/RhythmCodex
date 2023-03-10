using RhythmCodex.Graphics.Models;

namespace RhythmCodex.Plugin.ImageSharp;

public interface IImageSharpBitmapConverter
{
    Image<Rgba32> ConvertBitmap(IBitmap bitmap);
    IBitmap ConvertImage(Image<Rgba32> image);
}