using System.Collections.Generic;
using RhythmCodex.Games.Stepmania.Model;

namespace RhythmCodex.Games.Stepmania.Converters;

public interface ISmEncoder
{
    List<Command> Encode(ChartSet chartSet);
}