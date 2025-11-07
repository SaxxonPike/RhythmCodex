using System.Collections.Generic;
using System.IO;
using RhythmCodex.Beatmania.Ps2.Models;

namespace RhythmCodex.Beatmania.Ps2.Streamers;

public interface IBeatmaniaPs2NewChartEventStreamReader
{
    List<BeatmaniaPs2Event> Read(Stream stream, long length);
}