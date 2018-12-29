using System.IO;
using RhythmCodex.Tim.Models;

namespace RhythmCodex.Tim.Streamers
{
    public interface ITimStreamReader
    {
        TimImage Read(Stream stream);
    }
}