using System;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Psx.Heuristics;

[Service]
public class PsxExeHeuristic : IHeuristic
{
    public string Description => "Playstation Executable (PS-X EXE)";
    public string FileExtension => "exe";

    public HeuristicResult? Match(IHeuristicReader reader)
    {
        Span<byte> data = stackalloc byte[8];

        if (reader.Read(data) < 8)
            return null;

        if (Encodings.Cp437.GetString(data[..8]) != "PS-X EXE")
            return null;

        return new HeuristicResult(this);
    }
}