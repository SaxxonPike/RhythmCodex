using System;
using System.IO;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Mjpeg.Heuristics
{
    [Service]
    public class MjpegHeuristic : IHeuristic
    {
        public string Description => "MJPEG Movie";
        public string FileExtension => "mjpg";

        public HeuristicResult Match(ReadOnlySpan<byte> data)
        {
            if (data[0x0000] == 0x00 && data[0x0001] == 0x00)
                return null;
            
            if ((data[0x0000] & 0x1F) != 0)
                return null;

            if (data[0x0001] >= 0x20)
                return null;

            if (data[0x0002] != 0x00)
                return null;

            if (data[0x0003] != 0x38)
                return null;

            if (data[0x2000] == 0x00 && data[0x2001] == 0x00)
                return null;

            if ((data[0x2000] & 0x1F) != 0)
                return null;

            if (data[0x2001] >= 0x20)
                return null;

            if (data[0x2002] != 0x00)
                return null;

            if (data[0x2003] != 0x38)
                return null;
            
            return new HeuristicResult(this);
        }

        public HeuristicResult Match(Stream stream)
        {
            throw new NotImplementedException();
        }

        public int MinimumLength => 0x4000;
    }
}