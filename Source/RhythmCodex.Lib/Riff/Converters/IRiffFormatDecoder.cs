using RhythmCodex.Riff.Models;

namespace RhythmCodex.Riff.Converters;

public interface IRiffFormatDecoder
{
    RiffFormat Decode(RiffChunk chunk);
}