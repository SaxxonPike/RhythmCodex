using System.Collections.Generic;
using RhythmCodex.Charting;

namespace RhythmCodex.Bms.Converters
{
    public interface IBmsEncoder
    {
        IList<string> Encode(IChart chart);
    }
}