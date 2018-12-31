using RhythmCodex.Infrastructure;

namespace RhythmCodex.Wav.Models
{
    [Model]
    public class WaveFmtChunk
    {
        public int Format { get; set; }
        public int Channels { get; set; }
        public int SampleRate { get; set; }
        public int ByteRate { get; set; }
        public int BlockAlign { get; set; }
        public int BitsPerSample { get; set; }
    }
}