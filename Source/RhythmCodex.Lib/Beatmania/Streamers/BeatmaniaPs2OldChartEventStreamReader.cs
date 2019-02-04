using System.Collections.Generic;
using System.IO;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Beatmania.Streamers
{
    [Service]
    public class BeatmaniaPs2OldChartEventStreamReader : IBeatmaniaPs2OldChartEventStreamReader
    {
        public IList<BeatmaniaPs2Event> Read(Stream stream, int length)
        {
            throw new System.NotImplementedException();
        }
    }
}