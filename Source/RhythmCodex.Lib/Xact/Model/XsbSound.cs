using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model
{
    [Model]
    public struct XsbSound
    {
        public byte Flags { get; set; }
        public short Category { get; set; }
        public byte Volume { get; set; } // decibels
        public short Pitch { get; set; } // steps of 1000
        public byte Priority { get; set; }
        public short Unk0 { get; set; }
        
        public short TrackIndex { get; set; }
        public byte WaveBankIndex { get; set; }

        public XsbSoundRpc Rpc { get; set; }
        public XsbSoundDsp Dsp { get; set; }
        public XsbSoundClip[] Clips { get; set; }
    }
}