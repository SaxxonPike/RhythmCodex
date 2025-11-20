using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.Charts.Ssq.Streamers;
using RhythmCodex.Heuristics;
using RhythmCodex.IoC;

namespace RhythmCodex.Charts.Ssq;

[Service]
public class SsqHeuristic(ISsqStreamReader ssqStreamReader) : IReadableHeuristic<IEnumerable<SsqChunk>>
{
    public IEnumerable<SsqChunk> Read(HeuristicResult result, Stream stream)
    {
        return ssqStreamReader.Read(stream);
    }

    public string Description => "DDR Step Sequence";
    public string FileExtension => "SSQ";
        
    public HeuristicResult? Match(IHeuristicReader reader)
    {
        Span<byte> data = stackalloc byte[12];

        if (reader.Read(data) < 12)
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