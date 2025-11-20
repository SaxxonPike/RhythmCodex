using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Sm.Model;

namespace RhythmCodex.Charts.Sm.Converters;

public interface ISmEncoder
{
    List<Command> Encode(ChartSet chartSet);
}