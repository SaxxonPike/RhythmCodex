using System.Collections.Generic;
using RhythmCodex.Bms.Model;

namespace RhythmCodex.Bms.Converters
{
    public interface IBmsRandomResolver
    {
        IEnumerable<BmsCommand> Resolve(IEnumerable<BmsCommand> commands);
    }
}