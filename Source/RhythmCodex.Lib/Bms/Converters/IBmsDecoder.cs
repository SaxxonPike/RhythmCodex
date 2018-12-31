using System.Collections.Generic;
using RhythmCodex.Bms.Model;
using RhythmCodex.Charting;

namespace RhythmCodex.Bms.Converters
{
    public interface IBmsDecoder
    {
        BmsChart Decode(IEnumerable<BmsCommand> commands);
    }
}