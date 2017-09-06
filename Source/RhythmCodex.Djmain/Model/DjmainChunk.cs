using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model
{
    [Model]
    public class DjmainChunk : IDjmainChunk
    {
        public byte[] Data { get; set; }
        public int Id { get; set; }
    }
}
