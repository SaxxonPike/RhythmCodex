using System.IO;
using RhythmCodex.Graphics.Tga.Models;

namespace RhythmCodex.Graphics.Tga.Streamers;

public interface ITgaStreamReader
{
    TgaImage Read(Stream source, long length);
}