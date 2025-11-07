using System.Collections.Generic;
using RhythmCodex.Games.Stepmania.Model;

namespace RhythmCodex.Games.Stepmania.Converters;

public interface ISmDecoder
{
    ChartSet Decode(IEnumerable<Command> commands);
}