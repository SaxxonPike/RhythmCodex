using System.IO;
using RhythmCodex.Sif.Models;

namespace RhythmCodex.Sif.Streamers
{
    public interface ISifStreamReader
    {
        SifInfo Read(Stream stream, long length);
    }
}