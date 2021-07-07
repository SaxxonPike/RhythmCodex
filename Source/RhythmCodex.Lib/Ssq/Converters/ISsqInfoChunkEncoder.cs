using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ISsqInfoChunkEncoder
    {
        SsqChunk Encode(SsqInfoChunk infoChunk);
    }
}