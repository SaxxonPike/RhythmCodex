using System.IO;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model
{
    [Model]
    public struct WaveBankRegion
    {
        public int Offset { get; set; }
        public int Length { get; set; }
    }
}