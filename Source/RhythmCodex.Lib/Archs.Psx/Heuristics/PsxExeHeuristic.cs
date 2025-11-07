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
        var data = reader.Read(8);
        if (data.Length < 8)
            return null;

        if (Encodings.Cp437.GetString(data[..8]) != "PS-X EXE")
            return null;
            
        return new HeuristicResult(this);
    }
}