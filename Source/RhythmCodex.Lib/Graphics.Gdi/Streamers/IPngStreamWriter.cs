using System.IO;
using RhythmCodex.Graphics.Models;

namespace RhythmCodex.Graphics.Gdi.Streamers;

public interface IPngStreamWriter
{
    void Write(Stream stream, Bitmap bitmap);
}