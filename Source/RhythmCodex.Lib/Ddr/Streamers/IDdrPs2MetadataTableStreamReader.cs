using System.Collections.Generic;
using System.IO;
using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Streamers;

public interface IDdrPs2MetadataTableStreamReader
{
    List<DdrPs2MetadataTableEntry> Get(Stream stream, long length);
}