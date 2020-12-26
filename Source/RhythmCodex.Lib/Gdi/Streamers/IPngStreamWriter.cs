using System.IO;
using RhythmCodex.Graphics.Models;

namespace RhythmCodex.Gdi.Streamers
{
    public interface IPngStreamWriter
    {
        void Write(Stream stream, IRawBitmap rawBitmap);
    }
}