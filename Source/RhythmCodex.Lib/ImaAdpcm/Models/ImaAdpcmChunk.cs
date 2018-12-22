using RhythmCodex.Infrastructure;

namespace RhythmCodex.ImaAdpcm.Models
{
    [Model]
    public class ImaAdpcmChunk
    {
        public byte[] Data { get; set; }
        public int Channels { get; set; }
        public int Rate { get; set; }
        public int ChannelSamplesPerFrame { get; set; }
    }
}