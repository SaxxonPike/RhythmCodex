using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Psx
{
    [Service]
    public class PsxExeHeuristic : IHeuristic
    {
        public string Description => "Playstation Executable (PS-X EXE)";
        public string FileExtension => "exe";
        
        public bool IsMatch(ReadOnlySpan<byte> data)
        {
            if (data.Length < 8)
                return false;

            return Encodings.CP437.GetString(data.Slice(0, 8)) == "PS-X EXE";
        }
    }
}