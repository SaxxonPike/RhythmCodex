using System.IO;
using RhythmCodex.Graphics.Models;

namespace RhythmCodex.Graphics.Gdi.Streamers;

public interface IBitmapStreamWriter
{
    void Write(Stream stream, Bitmap bitmap);
}