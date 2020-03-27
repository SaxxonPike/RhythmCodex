using System.Collections.Generic;
using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Converters
{
    public interface IDdrPs2FileDataTableDecoder
    {
        IList<DdrPs2FileDataTableEntry> Decode(byte[] data);
    }
}