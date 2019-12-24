using System.Collections.Generic;
using System.IO;
using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Streamers
{
    public interface IMusicDbXmlStreamReader
    {
        IEnumerable<MusicDbEntry> Read(Stream stream);
    }
}