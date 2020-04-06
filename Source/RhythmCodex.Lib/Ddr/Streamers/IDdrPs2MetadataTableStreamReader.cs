using System.Collections.Generic;
using System.IO;
using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Streamers
{
    public interface IDdrPs2MetadataTableStreamReader
    {
        IList<DdrPs2MetadataTableEntry> Get(Stream stream, long length);
    }
}