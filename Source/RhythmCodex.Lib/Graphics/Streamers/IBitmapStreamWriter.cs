using System.IO;
using RhythmCodex.Graphics.Models;

namespace RhythmCodex.Graphics.Streamers;

public interface IBitmapStreamWriter
{
    void Write(Stream stream, Bitmap bitmap);
}