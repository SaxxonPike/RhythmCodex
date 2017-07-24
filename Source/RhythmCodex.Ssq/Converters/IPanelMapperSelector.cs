using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface IPanelMapperSelector
    {
        IPanelMapper Select(IEnumerable<Step> steps);
    }
}