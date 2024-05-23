using System.Collections.Generic;
using RhythmCodex.Bms.Model;

namespace RhythmCodex.Bms.Converters;

public interface IBmsDecoder
{
    BmsChart Decode(IEnumerable<BmsCommand> commands);
}