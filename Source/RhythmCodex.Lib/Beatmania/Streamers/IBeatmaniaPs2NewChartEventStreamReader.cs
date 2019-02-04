using System.Collections.Generic;
using System.IO;
using RhythmCodex.Beatmania.Models;

namespace RhythmCodex.Beatmania.Streamers
{
    public interface IBeatmaniaPs2NewChartEventStreamReader
    {
        IList<BeatmaniaPs2Event> Read(Stream stream, int length);
    }
}