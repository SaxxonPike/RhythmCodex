using System.IO;
using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Streamers
{
    public interface ISifStreamReader
    {
        SifInfo Read(Stream stream, long length);
    }
}