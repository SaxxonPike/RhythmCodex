using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model
{
    [Model]
    public struct XsbSoundClip
    {
        public byte Volume { get; set; }
        public int ClipOffset { get; set; }
        
    }

    [Model]
    public struct XsbSoundClipEvent
    {
        
    }
}