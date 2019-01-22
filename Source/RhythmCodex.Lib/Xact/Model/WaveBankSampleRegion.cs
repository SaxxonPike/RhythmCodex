using System.IO;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model
{
    [Model]
    public struct WaveBankSampleRegion
    {
        public int StartSample { get; set; }
        public int TotalSamples { get; set; }
    }
}