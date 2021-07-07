using System.Collections.Generic;
using System.IO;
using RhythmCodex.Heuristics;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;
using RhythmCodex.Ssq.Streamers;

namespace RhythmCodex.Ssq
{
    [Service]
    public class SsqHeuristic : IReadableHeuristic<IEnumerable<SsqChunk>>
    {
        private readonly ISsqStreamReader _ssqStreamReader;

        public SsqHeuristic(ISsqStreamReader ssqStreamReader)
        {
            _ssqStreamReader = ssqStreamReader;
        }

        public IEnumerable<SsqChunk> Read(HeuristicResult heuristicResult, Stream stream)
        {
            return _ssqStreamReader.Read(stream);
        }

        public string Description => "DDR Step Sequence";
        public string FileExtension => "SSQ";
        
        public HeuristicResult Match(IHeuristicReader reader)
        {
            var data = reader.Read(12);
            if (data.Length < 12)
                return null;

            if (data[0] % 4 != 0)
                return null;

            if (data[0] == 0 && data[1] == 0)
                return null;

            if (data[2] != 0)
                return null;

            if (data[3] != 0)
                return null;

            if (data[4] != 1)
                return null;

            if (data[5] != 0)
                return null;

            if (data[6] == 0 && data[7] == 0)
                return null;

            if (data[8] == 0 && data[9] == 0)
                return null;

            if (data[10] != 0)
                return null;

            if (data[11] != 0)
                return null;

            return new HeuristicResult(this);
        }
    }
}