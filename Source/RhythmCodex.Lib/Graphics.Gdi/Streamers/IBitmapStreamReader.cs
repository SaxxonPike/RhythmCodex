using System.IO;
using RhythmCodex.Graphics.Models;

namespace RhythmCodex.Graphics.Gdi.Streamers;

public interface IBitmapStreamReader
{
    Bitmap Read(Stream stream);
}