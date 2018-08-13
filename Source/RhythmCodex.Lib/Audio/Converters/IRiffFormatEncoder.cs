using RhythmCodex.Audio.Models;

namespace RhythmCodex.Audio.Converters
{
    public interface IRiffFormatEncoder
    {
        IRiffChunk Encode(IRiffFormat format);
    }
}