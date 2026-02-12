using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archs.Firebeat.Models;

public class FirebeatArchive
{
    public FirebeatChunk Chunk { get; set; }

    public int Id => Chunk.Id;
    
    public List<Chart> Charts { get; set; } = [];

    public List<Sound> Samples { get; set; } = [];

    public List<FirebeatChart> RawCharts { get; set; } = [];

    public Dictionary<int, FirebeatSampleInfo> RawSampleInfos { get; set; } = [];

    public override string ToString() => Json.Serialize(this);
}