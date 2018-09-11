using RhythmCodex.Riff.Models;

namespace RhythmCodex.Riff.Converters
{
    public interface IRiffFormatEncoder
    {
        IRiffChunk Encode(IRiffFormat format);
    }
}