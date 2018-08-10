using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model
{
    [Model]
    public class DjmainChunk : IDjmainChunk
    {
        public DjmainChunkFormat Format { get; set; }
        public byte[] Data { get; set; }
        public int Id { get; set; }
    }
}