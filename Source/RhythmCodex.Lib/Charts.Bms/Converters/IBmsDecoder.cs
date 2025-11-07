using System.Collections.Generic;
using RhythmCodex.Charts.Bms.Model;

namespace RhythmCodex.Charts.Bms.Converters;

public interface IBmsDecoder
{
    BmsChart Decode(IEnumerable<BmsCommand> commands);
}