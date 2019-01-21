using System.IO;
using RhythmCodex.Tga.Models;

namespace RhythmCodex.Tga.Streamers
{
    public interface ITgaStreamReader
    {
        TgaImage Read(Stream source, long length);
    }
}