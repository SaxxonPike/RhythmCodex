using System;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Twinkle.Heuristics;

[Service]
public class TwinkleBeatmaniaChartHeuristic : ITwinkleBeatmaniaChartHeuristic
{
    public string Description => "Twinkle Beatmania Chart";

    public string FileExtension => ".ac7";

    public HeuristicResult? Match(IHeuristicReader reader)
    {
        var noteCountMode = true;
        var hasBpm = false;
        var hasEnd = false;
        var hasTerminator = false;
        Span<byte> evData = stackalloc byte[4];

        while (true)
        {
            if (reader.Read(evData) < 4)
                break;

            var eventOffset = Bitter.ToInt16S(evData);
            var eventValue = evData[2];
            var eventCommand = evData[3];
            var eventParameter = eventCommand >> 4;
            var eventType = eventCommand & 0xF;

            // empty event = invalid
            if (Bitter.ToInt32(evData) == 0)
                return null;

            // positive event offsets only
            if (eventOffset < 0)
                return null;

            // offsets can't be present during note count
            if (noteCountMode && eventOffset != 0)
                return null;

            // disable note count info if another event type shows up
            if (eventCommand != 0x00 && eventCommand != 0x01)
                noteCountMode = false;

            // skip the rest of processing if in note count mode
            if (noteCountMode)
                continue;

            // terminator bytes
            if (eventOffset == 0x7FFF)
            {
                hasTerminator = true;
                break;
            }

            // make sure we have the bare minimums
            if (eventType == 6)
                hasEnd = true;
            else if (eventType == 4 && eventValue + (eventParameter << 8) != 0)
                hasBpm = true;
        }

        if (!(hasBpm && hasEnd && hasTerminator))
            return null;

        return new HeuristicResult(this);
    }
}