using System.Collections.Generic;
using System.IO;
using RhythmCodex.Beatmania.Ps2.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Beatmania.Ps2.Streamers;

[Service]
public class BeatmaniaPs2NewChartEventStreamReader : IBeatmaniaPs2NewChartEventStreamReader
{
    public List<BeatmaniaPs2Event> Read(Stream stream, long length)
    {
        throw new System.NotImplementedException();
    }
}