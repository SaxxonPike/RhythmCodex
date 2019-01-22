using RhythmCodex.Infrastructure;

namespace RhythmCodex.Beatmania.Models
{
    [Model]
    public class BeatmaniaPcAudioEntry
    {
        public int Reserved { get; set; }
        public int Channel { get; set; }
        public int Panning { get; set; }
        public int Volume { get; set; }
        public byte[] ExtraInfo { get; set; }
        public byte[] Data { get; set; }
    }
}