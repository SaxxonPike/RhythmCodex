﻿using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Mappers;

public interface IPanelMapperSelector
{
    IPanelMapper Select(IEnumerable<Step> steps, ChartInfo chartInfo);
}