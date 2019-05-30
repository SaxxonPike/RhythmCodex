using System.IO;
using RhythmCodex.Graphics.Models;

namespace RhythmCodex.Gdi.Streamers
{
    public interface IBitmapStreamReader
    {
        RawBitmap Read(Stream stream);
    }
}