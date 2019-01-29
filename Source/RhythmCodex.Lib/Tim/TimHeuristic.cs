using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Tim
{
    [Service]
    public class TimHeuristic : IHeuristic
    {
        public string Description => "Playstation TIM image";
        public string FileExtension => "TIM";
        
        public bool IsMatch(ReadOnlySpan<byte> data)
        {
            if (data.Length < 8)
                return false;

            if (data[0] != 0x10)
                return false;
            if (data[1] != 0x00)
                return false;
            if (data[2] != 0x00)
                return false;
            if (data[3] != 0x00)
                return false;
            
            switch (data[4])
            {
                case 0x00: break;
                case 0x01: break;
                case 0x02: break;
                case 0x08: break;
                case 0x09: break;
                default: return false;
            }
            
            if (data[5] != 0x00)
                return false;
            if (data[6] != 0x00)
                return false;
            if (data[7] != 0x00)
                return false;

            return true;
        }
    }
}