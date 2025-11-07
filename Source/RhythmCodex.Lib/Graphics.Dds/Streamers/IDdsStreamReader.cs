using System.IO;
using RhythmCodex.Graphics.Dds.Models;

namespace RhythmCodex.Graphics.Dds.Streamers;

public interface IDdsStreamReader
{
    DdsImage Read(Stream source, int length);
}