using System.IO;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model
{
    [Model]
    public struct WaveBankHeader
    {
        public int Signature { get; set; }
        public int Version { get; set; }
        public int HeaderVersion { get; set; }
        public WaveBankRegion[] Segments { get; set; }
    }
}