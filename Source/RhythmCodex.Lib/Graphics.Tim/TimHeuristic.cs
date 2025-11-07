using System.IO;
using RhythmCodex.Graphics.Tim.Models;
using RhythmCodex.Graphics.Tim.Streamers;
using RhythmCodex.Heuristics;
using RhythmCodex.IoC;

namespace RhythmCodex.Graphics.Tim;

[Service]
public class TimHeuristic(ITimStreamReader timStreamReader) : IReadableHeuristic<TimImage>
{
    public TimImage Read(HeuristicResult result, Stream stream)
    {
        return timStreamReader.Read(stream);
    }

    public string Description => "Playstation TIM image";
    public string FileExtension => "TIM";

    public HeuristicResult? Match(IHeuristicReader reader)
    {
        var data = reader.Read(8);
        if (data.Length < 8)
            return null;

        if (data[0] != 0x10)
            return null;
        if (data[1] != 0x00)
            return null;
        if (data[2] != 0x00)
            return null;
        if (data[3] != 0x00)
            return null;
            
        switch (data[4])
        {
            case 0x00: break;
            case 0x01: break;
            case 0x02: break;
            case 0x08: break;
            case 0x09: break;
            default: return null;
        }
            
        if (data[5] != 0x00)
            return null;
        if (data[6] != 0x00)
            return null;
        if (data[7] != 0x00)
            return null;
            
        return new HeuristicResult(this);
    }
}