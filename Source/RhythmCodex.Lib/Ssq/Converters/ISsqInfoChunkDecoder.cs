using JetBrains.Annotations;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[PublicAPI]
public interface ISsqInfoChunkDecoder
{
    SsqInfoChunk Decode(SsqChunk ssqChunk);
}