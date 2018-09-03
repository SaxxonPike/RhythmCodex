using System.Collections.Generic;
using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Converters
{
    public interface IDdr573Decoder
    {
        IList<Ddr573File> Decode(Ddr573Image image);
    }
}