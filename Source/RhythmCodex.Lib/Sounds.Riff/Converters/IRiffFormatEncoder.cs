using RhythmCodex.Sounds.Riff.Models;

namespace RhythmCodex.Sounds.Riff.Converters;

public interface IRiffFormatEncoder
{
    RiffChunk Encode(RiffFormat format);
}