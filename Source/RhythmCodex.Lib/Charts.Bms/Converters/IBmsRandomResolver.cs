using System.Collections.Generic;
using RhythmCodex.Charts.Bms.Model;

namespace RhythmCodex.Charts.Bms.Converters;

public interface IBmsRandomResolver
{
    List<BmsCommand> Resolve(IEnumerable<BmsCommand> commands);
}