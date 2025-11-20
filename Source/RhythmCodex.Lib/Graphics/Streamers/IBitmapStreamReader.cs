using System.IO;
using RhythmCodex.Graphics.Models;

namespace RhythmCodex.Graphics.Streamers;

public interface IBitmapStreamReader
{
    Bitmap Read(Stream stream);
}