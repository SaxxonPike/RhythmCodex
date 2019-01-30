using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Ssq
{
    [Service]
    public class SsqHeuristic : IHeuristic
    {
        public string Description => "DDR Step Sequence";
        public string FileExtension => "SSQ";
        
        public bool IsMatch(ReadOnlySpan<byte> data)
        {
            if (data.Length < 12)
                return false;

            if (data[0] % 4 != 0)
                return false;

            if (data[0] == 0 && data[1] == 0)
                return false;

            if (data[2] != 0)
                return false;

            if (data[3] != 0)
                return false;

            if (data[4] != 1)
                return false;

            if (data[5] != 0)
                return false;

            if (data[6] == 0 && data[7] == 0)
                return false;

            if (data[8] == 0 && data[9] == 0)
                return false;

            if (data[10] != 0)
                return false;

            if (data[11] != 0)
                return false;

            if (data[data.Length - 1] != 0)
                return false;

            if (data[data.Length - 2] != 0)
                return false;

            if (data[data.Length - 3] != 0)
                return false;

            if (data[data.Length - 4] != 0)
                return false;

            return true;
        }
    }
}