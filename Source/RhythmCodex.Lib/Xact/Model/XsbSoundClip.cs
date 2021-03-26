using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model
{
    [Model]
    public struct XsbSoundClip
    {
        public byte Volume { get; set; }
        public int ClipOffset { get; set; }
        public short FilterFlags { get; set; }
        public short FilterFrequency { get; set; }
        public XsbSoundClipEvent[] Events { get; set; }
    }

    [Model]
    public struct XsbSoundClipEvent
    {
        public int EventId
        {
            get => Info & 0x1F;
            set => Info = (Info & ~0x1F) | (value & 0x1F);
        }

        public int TimeStamp => (Info >> 5) & 0xFFFF; // units of 0.001
        public bool PlayRelease => (Flags1 & 0x01) != 0x00;
        public bool PanEnabled => (Flags1 & 0x02) != 0x00;
        public bool UseCenterSpeaker => (Flags1 & 0x04) != 0x00;

        public int Info { get; set; }
        public short RandomOffset { get; set; } // units of 0.001

        public byte Flags0 { get; set; }
        public byte Flags1 { get; set; }
        public byte LoopCount { get; set; }
        public short PanAngle { get; set; } // units of 0.01
        public short PanArc { get; set; } // units of 0.01

        public short TrackIndex { get; set; }
        public byte WaveBankIndex { get; set; }
    }
}