using System.Buffers.Binary;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Sounds.Xact.Heuristics;

[Service]
public class XwbHeuristic : IHeuristic
{
    public string Description => "Xbox Wave Bank";
    public string FileExtension => "xwb";

    public HeuristicResult? Match(IHeuristicReader reader)
    {
        var data = reader.Read(4);
        if (data.Length < 4)
            return null;

        var value = BinaryPrimitives.ReadInt32LittleEndian(data);
        return (value == 0x444E4257)
            ? new HeuristicResult(this)
            : null;
    }
}