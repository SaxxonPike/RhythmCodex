using System;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Vag.Heuristics;

namespace RhythmCodex.Beatmania.Heuristics
{
    [Service]
    public class BeatmaniaPs2OldBgmHeuristic : IHeuristic
    {
        public string Description => "BeatmaniaIIDX CS BGM (old)";
        public string FileExtension => "bmcsbgm";
        
        public HeuristicResult Match(ReadOnlySpan<byte> data)
        {
            if (data.Length < MinimumLength)
                return null;

            var length = Bitter.ToInt32S(data);
            
            if (length == 0)
                return null;

            if ((length & 0x7FF) != 0)
                return null;

            if (data[0x05] == 0)
                return null;

            var sampleRate = Bitter.ToInt16S(data, 6);
            if (sampleRate == 0)
                return null;

            if (data[0x08] == 0)
                return null;

            if (data[0x08] > 2)
                return null;

            return new VagHeuristicResult(this)
            {
                Start = 0x800,
                Interval = 0x800,
                Channels = data[0x08],
                SampleRate = sampleRate,
                Length = length
            };
        }

        public int MinimumLength => 0x10;
    }
}