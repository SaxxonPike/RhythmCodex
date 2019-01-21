using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ISsqInfoChunkDecoder
    {
        SsqInfoChunk Decode(Chunk chunk);
    }
}