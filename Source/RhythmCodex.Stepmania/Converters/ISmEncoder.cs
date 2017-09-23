using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    public interface ISmEncoder
    {
        IEnumerable<Command> Encode(ChartSet chartSet);
    }
}