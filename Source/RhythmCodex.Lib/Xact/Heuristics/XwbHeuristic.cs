using System;
using System.IO;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Xact.Heuristics
{
    [Service]
    public class XwbHeuristic : IHeuristic
    {
        public string Description => "Xbox Wave Bank";
        public string FileExtension => "xwb";

        public HeuristicResult Match(IHeuristicReader reader)
        {
            var data = reader.Read(4);
            if (data.Length < 4)
                return null;

            if (data[0x00] != 0x57)
                return null;

            if (data[0x01] != 0x42)
                return null;

            if (data[0x02] != 0x4E)
                return null;

            if (data[0x03] != 0x44)
                return null;
            
            return new HeuristicResult(this);
        }
    }
}