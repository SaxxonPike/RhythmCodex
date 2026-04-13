using System;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Psx.Heuristics;

[Service]
public sealed class PsxExeHeuristic : IHeuristic
{
    public string Description => "Playstation Executable (PS-X EXE)";
    public string FileExtension => "exe";

    public HeuristicResult? Match(IHeuristicReader reader)
    {
        Span<byte> data = stackalloc byte[8];

        if (reader.Read(data) < 8 ||
            !data[..8].SequenceEqual("PS-X EXE"u8))
            return null;

        return new HeuristicResult(this);
    }
}