using System.Collections.Generic;

namespace RhythmCodex.Ssq.Converters
{
    public interface IStepPanelSplitter
    {
        IList<int> Split(int panels);
    }
}