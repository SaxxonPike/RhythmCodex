using System.IO;
using RhythmCodex.Graphics.Models;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Gdi.Streamers
{
    public interface IBitmapStreamReader
    {
        RawBitmap Read(Stream stream);
    }
}