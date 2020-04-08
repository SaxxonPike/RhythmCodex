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

        public HeuristicResult Match(IHeuristicReader reader)
        {
            var data = reader.Read(0x4);

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

            reader.Skip(0x1FFC);
            
            var data2 = reader.Read(0x4);

            if (data2[0x0000] == 0x00 && data2[0x0001] == 0x00)
                return null;

            if ((data2[0x0000] & 0x1F) != 0)
                return null;

            if (data2[0x0001] >= 0x20)
                return null;

            if (data2[0x0002] != 0x00)
                return null;

            if (data2[0x0003] != 0x38)
                return null;

            return new HeuristicResult(this);
        }
    }
}