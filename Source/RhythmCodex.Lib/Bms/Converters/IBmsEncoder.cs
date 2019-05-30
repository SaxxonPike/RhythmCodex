using System.Collections.Generic;
using RhythmCodex.Bms.Model;
using RhythmCodex.Charting.Models;

namespace RhythmCodex.Bms.Converters
{
    public interface IBmsEncoder
    {
        IList<BmsCommand> Encode(IChart chart);
    }
}