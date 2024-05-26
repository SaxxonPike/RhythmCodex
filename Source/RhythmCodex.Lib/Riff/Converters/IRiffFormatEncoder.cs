using RhythmCodex.Riff.Models;

namespace RhythmCodex.Riff.Converters;

public interface IRiffFormatEncoder
{
    RiffChunk Encode(RiffFormat format);
}