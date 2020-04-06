using System;
using System.IO;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Twinkle.Heuristics
{
    [Service]
    public class TwinkleBeatmaniaChartHeuristic : ITwinkleBeatmaniaChartHeuristic
    {
        public string Description { get; }

        public string FileExtension { get; }

        public HeuristicResult Match(ReadOnlySpan<byte> data)
        {
            var offset = 0;
            var length = data.Length & ~0x3;
            var noteCountMode = true;
            var hasBpm = false;
            var hasEnd = false;
            var hasTerminator = false;

            while (offset < length)
            {
                var eventOffset = Bitter.ToInt16S(data, offset);
                var eventValue = data[offset + 2];
                var eventCommand = data[offset + 3];
                var eventParameter = eventCommand >> 4;
                var eventType = eventCommand & 0xF;

                // empty event = invalid
                if (Bitter.ToInt32(data, offset) == 0)
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
                {
                    offset += 4;
                    continue;
                }

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

                offset += 4;
            }

            if (!(hasBpm && hasEnd && hasTerminator))
                return null;
            
            return new HeuristicResult(this);
        }

        public HeuristicResult Match(Stream stream)
        {
            throw new NotImplementedException();
        }

        public int MinimumLength => 12;
    }

    public interface ITwinkleBeatmaniaChartHeuristic : IHeuristic
    {
    }
}