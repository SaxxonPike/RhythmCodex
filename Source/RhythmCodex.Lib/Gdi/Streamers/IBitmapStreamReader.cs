using System.IO;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Gdi.Streamers
{
    public interface IBitmapStreamReader
    {
        RawBitmap Read(Stream stream);
    }
}