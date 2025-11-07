using System.Collections.Generic;
using System.IO;
using RhythmCodex.Games.Ddr.Ps2.Models;

namespace RhythmCodex.Games.Ddr.Streamers;

public interface IDdrPs2MetadataTableStreamReader
{
    List<DdrPs2MetadataTableEntry> Get(Stream stream, long length);
}