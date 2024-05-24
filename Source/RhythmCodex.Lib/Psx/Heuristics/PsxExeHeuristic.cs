using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Psx.Heuristics;

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

        if (Encodings.CP437.GetString(data.Slice(0, 8)) != "PS-X EXE")
            return null;
            
        return new HeuristicResult(this);
    }
}