using System;
using System.IO;
using RhythmCodex.Heuristics;
using RhythmCodex.IoC;
using RhythmCodex.Tim.Models;
using RhythmCodex.Tim.Streamers;

namespace RhythmCodex.Tim
{
    [Service]
    public class TimHeuristic : IReadableHeuristic<TimImage>
    {
        private readonly ITimStreamReader _timStreamReader;

        public TimHeuristic(ITimStreamReader timStreamReader)
        {
            _timStreamReader = timStreamReader;
        }
        
        public TimImage Read(HeuristicResult heuristicResult, Stream stream)
        {
            return _timStreamReader.Read(stream);
        }

        public string Description => "Playstation TIM image";
        public string FileExtension => "TIM";
        
        public HeuristicResult Match(ReadOnlySpan<byte> data)
        {
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

        public HeuristicResult Match(Stream stream)
        {
            throw new NotImplementedException();
        }

        public int MinimumLength => 8;
    }
}