using System.Collections.Generic;

namespace RhythmCodex.Archs.Firebeat.Models;

public class FirebeatSampleSet
{
    public Dictionary<int, FirebeatSampleInfo> Infos { get; set; } = [];
    public Dictionary<int, FirebeatSample> Samples { get; set; } = [];
}