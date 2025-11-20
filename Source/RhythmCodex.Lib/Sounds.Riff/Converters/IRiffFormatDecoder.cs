using RhythmCodex.Sounds.Riff.Models;

namespace RhythmCodex.Sounds.Riff.Converters;

public interface IRiffFormatDecoder
{
    RiffFormat Decode(RiffChunk chunk);
}