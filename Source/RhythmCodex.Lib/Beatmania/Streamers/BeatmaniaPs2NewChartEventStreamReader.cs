using System.Collections.Generic;
using System.IO;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Beatmania.Streamers;

[Service]
public class BeatmaniaPs2NewChartEventStreamReader : IBeatmaniaPs2NewChartEventStreamReader
{
    public List<BeatmaniaPs2Event> Read(Stream stream, long length)
    {
        throw new System.NotImplementedException();
    }
}