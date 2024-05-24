using System.Collections.Generic;
using System.IO;
using RhythmCodex.Beatmania.Models;

namespace RhythmCodex.Beatmania.Streamers;

public interface IBeatmaniaPs2NewChartEventStreamReader
{
    List<BeatmaniaPs2Event> Read(Stream stream, long length);
}