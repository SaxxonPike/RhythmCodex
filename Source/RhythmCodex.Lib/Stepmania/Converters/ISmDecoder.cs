﻿using System.Collections.Generic;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters;

public interface ISmDecoder
{
    ChartSet Decode(IEnumerable<Command> commands);
}