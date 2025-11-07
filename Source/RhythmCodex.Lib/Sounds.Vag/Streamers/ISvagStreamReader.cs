using System.IO;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Sounds.Vag.Streamers;

public interface ISvagStreamReader
{
    SvagContainer Read(Stream stream);
}