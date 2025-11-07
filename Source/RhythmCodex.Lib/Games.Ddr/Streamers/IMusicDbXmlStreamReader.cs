using System.Collections.Generic;
using System.IO;
using RhythmCodex.Games.Ddr.Models;

namespace RhythmCodex.Games.Ddr.Streamers;

public interface IMusicDbXmlStreamReader
{
    IEnumerable<MusicDbEntry> Read(Stream stream);
}