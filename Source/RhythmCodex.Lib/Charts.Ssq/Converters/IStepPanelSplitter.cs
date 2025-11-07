using System.Collections.Generic;
using JetBrains.Annotations;

namespace RhythmCodex.Charts.Ssq.Converters;

[PublicAPI]
public interface IStepPanelSplitter
{
    List<int> Split(int panels);
}