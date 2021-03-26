using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model
{
    [Model]
    public struct XsbHeader
    {
        public int Signature { get; set; }
        public short ToolVersion { get; set; }
        public short FormatVersion { get; set; }
        public short Crc { get; set; }
        public long BuildTime { get; set; }
        public byte Platform { get; set; }
        public short SimpleCueCount { get; set; }
        public short ComplexCueCount { get; set; }
        public short Unk0 { get; set; }
        public short TotalCueCount { get; set; }
        public byte WaveBankCount { get; set; }
        public short SoundCount { get; set; }
        public short CueNameTableLength { get; set; }
        public short Unk1 { get; set; }
        public int SimpleCuesOffset { get; set; }
        public int ComplexCuesOffset { get; set; }
        public int CueNamesOffset { get; set; }
        public int Unk2 { get; set; }
        public int VariationTablesOffset { get; set; }
        public int Unk3 { get; set; }
        public int WaveBankNameTableOffset { get; set; }
        public int CueNameHashTableOffset { get; set; }
        public int CueNameHashValuesOffset { get; set; }
        public int SoundsOffset { get; set; }
        public string Name { get; set; }
    }
}