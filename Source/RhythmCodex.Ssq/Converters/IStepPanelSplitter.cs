using System.Collections.Generic;

namespace RhythmCodex.Ssq.Converters
{
    public interface IStepPanelSplitter
    {
        IEnumerable<int> Split(int panels);
    }
}