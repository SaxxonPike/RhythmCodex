using System.Collections.Generic;
using System.IO;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Games.Beatmania.Ps2.Streamers;

[Service]
public class BeatmaniaPs2OldChartEventStreamReader : IBeatmaniaPs2OldChartEventStreamReader
{
    public List<BeatmaniaPs2Event> Read(Stream stream, long length)
    {
        throw new System.NotImplementedException();
    }
}