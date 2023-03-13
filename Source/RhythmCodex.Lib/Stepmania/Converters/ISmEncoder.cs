using System.Collections.Generic;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters;

public interface ISmEncoder
{
    IList<Command> Encode(ChartSet chartSet);
}