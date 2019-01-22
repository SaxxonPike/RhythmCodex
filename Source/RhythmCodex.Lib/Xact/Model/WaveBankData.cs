using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model
{
    [Model]
    public struct WaveBankData
    {
        public int Flags { get; set; }
        public int EntryCount { get; set; }
        public string BankName { get; set; }
        public int EntryMetaDataElementSize { get; set; }
        public int EntryNameElementSize { get; set; }
        public int Alignment { get; set; }
        public WaveBankMiniWaveFormat CompactFormat { get; set; }
        public long BuildTime { get; set; }
    }
}