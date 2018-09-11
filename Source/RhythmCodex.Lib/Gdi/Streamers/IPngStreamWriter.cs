using System.IO;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Gdi.Streamers
{
    public interface IPngStreamWriter
    {
        void Write(Stream stream, RawBitmap rawBitmap);
    }
}