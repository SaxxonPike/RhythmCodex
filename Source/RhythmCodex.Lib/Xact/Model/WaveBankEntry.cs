using System.IO;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model
{
    [Model]
    public struct WaveBankEntry
    {
        public int Value { get; set; }
        public WaveBankMiniWaveFormat Format { get; set; }
        public WaveBankRegion PlayRegion { get; set; }
        public WaveBankSampleRegion LoopRegion { get; set; }
        
        public int Flags => Value & 0xF;
        public int Duration => Value >> 4;
    }
}