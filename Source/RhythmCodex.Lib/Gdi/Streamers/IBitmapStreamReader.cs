using System.IO;
using RhythmCodex.Graphics.Models;

namespace RhythmCodex.Gdi.Streamers;

public interface IBitmapStreamReader
{
    Bitmap Read(Stream stream);
}