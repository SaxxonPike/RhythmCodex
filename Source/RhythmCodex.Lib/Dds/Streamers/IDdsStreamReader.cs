using System.IO;
using RhythmCodex.Dds.Models;

namespace RhythmCodex.Dds.Streamers;

public interface IDdsStreamReader
{
    DdsImage Read(Stream source, int length);
}