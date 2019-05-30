using System.Collections.Generic;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Digital573.Models;

namespace RhythmCodex.Ddr.Converters
{
    public interface IDdr573ImageDecoder
    {
        IList<Ddr573File> Decode(Ddr573Image image);
    }
}