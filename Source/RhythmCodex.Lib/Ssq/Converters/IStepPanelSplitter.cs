using System.Collections.Generic;
using JetBrains.Annotations;

namespace RhythmCodex.Ssq.Converters;

[PublicAPI]
public interface IStepPanelSplitter
{
    IList<int> Split(int panels);
}