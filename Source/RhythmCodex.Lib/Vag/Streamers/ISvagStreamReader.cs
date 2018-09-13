using System.IO;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Streamers
{
    public interface ISvagStreamReader
    {
        SvagContainer Read(Stream stream);
    }
}