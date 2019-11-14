using System.Collections.Generic;
using RhythmCodex.Bms.Model;

namespace RhythmCodex.Bms.Converters
{
    public interface IBmsRandomResolver
    {
        IList<BmsCommand> Resolve(IEnumerable<BmsCommand> commands);
    }
}