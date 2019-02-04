using System;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Beatmania.Heuristics
{
    [Service]
    [Context(Context.BeatmaniaIidxCs)]
    public class BeatmaniaPs2OldKeysoundHeuristic : IHeuristic
    {
        public string Description => "BeatmaniaIIDX CS Keysounds (old)";
        public string FileExtension => "bmcskey";

        public HeuristicResult Match(ReadOnlySpan<byte> data)
        {
            if (data.Length < MinimumLength)
                return null;

            var a = Bitter.ToInt32(data);
            var b = Bitter.ToInt32(data, 4);
            var c = Bitter.ToInt32(data, 8);
            var d = Bitter.ToInt32(data, 12);

            if (a <= 0)
                return null;

            if (b <= 0)
                return null;

            if (c < 0)
                return null;

            if (d != 0)
                return null;

            if (a - b - c != 16384)
                return null;

            return new HeuristicResult(this);
        }

        public int MinimumLength => 0x10;
    }
}